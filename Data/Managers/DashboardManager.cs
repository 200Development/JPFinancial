using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using JPFData.DTO;
using JPFData.Models;
using JPFData.ViewModels;


namespace JPFData.Managers
{
    public class DashboardManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();
        private readonly DatabaseEditor _dbEditor = new DatabaseEditor();


        public DashboardManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }


        public DashboardDTO Get()
        {
            return Get(new DashboardDTO());
        }

        public DashboardDTO Get(DashboardDTO entity)
        {
            DashboardDTO ret = new DashboardDTO();
            TransactionManager tManager = new TransactionManager();

            ret.Transactions = tManager.Get(new Transaction());
            ret.StaticFinancialMetrics = RefreshFinancialMetrics(ret);
            ret.TimePeriodMetrics = RefreshTimePeriodMetrics(ret);
            return ret;
        }

        private StaticFinancialMetrics RefreshFinancialMetrics(DashboardDTO entity)
        {
            StaticFinancialMetrics metrics = new StaticFinancialMetrics();

            //var firstDayOfMonth = _calculations.FirstDayOfMonth(DateTime.Today.Year, DateTime.Today.Month);
            //var firstPaycheck = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15); //ToDo: make dynamic
            //var lastPaycheck = _calculations.LastDayOfMonth(DateTime.Today);
            var income = _calculations.GetMonthlyIncome();
            var bills = _db.Bills.ToList();
            var loans = _db.Loans.ToList();

            var monthlyLoanInterest = 0.0m;
            var dailyLoanInterest = 0.0m;
            foreach (var loan in loans)
            {
                monthlyLoanInterest += _calculations.MonthlyInterest(loan);
                dailyLoanInterest += _calculations.DailyInterest(loan);
            }

            //TODO: difference between totalDue and billsDue
            //var billsDue = _calculations.BillsDue(firstPaycheck, firstDayOfMonth, new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastPaycheck));
            //var totalDue = billsDue.Sum(bill => Convert.ToDecimal(bill.AmountDue));
            var costliestExpense = entity.Transactions.Where(t => t.Date.Month == DateTime.Today.AddMonths(-1).Month).OrderByDescending(t => t.Amount).Select(t => t.Amount).Take(1).FirstOrDefault();

            metrics.MandatoryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => b.IsMandatory).Sum(b => b.AmountDue);
            metrics.DiscretionaryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => !b.IsMandatory).Sum(b => b.AmountDue);
            metrics.Expenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month && b.DueDate.Year == DateTime.Today.Year).Sum(b => b.AmountDue);
            metrics.LastMonthExpenses = _calculations.LastMonthsExpenses();
            metrics.CostliestExpenseAmount = costliestExpense;
            metrics.CostliestCategory = entity.Transactions.Where(t => t.Date.Month == DateTime.Today.AddMonths(-1).Month).OrderByDescending(t => t.Amount).Select(t => t.Category).Take(1).FirstOrDefault();



            metrics.CostliestExpensePercentage = costliestExpense / income;
            metrics.LoanInterestPercentOfIncome = monthlyLoanInterest / income;
            metrics.MonthlyLoanInterest = monthlyLoanInterest;
            metrics.DailyLoanInterestPercentage = dailyLoanInterest / income;
            metrics.DailyLoanInterest = dailyLoanInterest;


            return metrics;
        }

        private TimePeriodFinancialMetrics RefreshTimePeriodMetrics(DashboardDTO entity)
        {
            TimePeriodFinancialMetrics metric = new TimePeriodFinancialMetrics();

            metric.OneMonthSavings = _calculations.FutureValue(DateTime.Today.AddMonths(1), entity.StaticFinancialMetrics.NetIncome);
            metric.ThreeMonthsSavings = _calculations.FutureValue(DateTime.Today.AddMonths(3), entity.StaticFinancialMetrics.NetIncome);
            metric.SixMonthsSavings = _calculations.FutureValue(DateTime.Today.AddMonths(6), entity.StaticFinancialMetrics.NetIncome);
            metric.OneYearSavings = _calculations.FutureValue(DateTime.Today.AddYears(1), entity.StaticFinancialMetrics.NetIncome);
            metric.MonthlyExpenses = entity.StaticFinancialMetrics.TotalDue;
            metric.MonthlyIncome = (Convert.ToDecimal(_db.Salaries.Select(s => s.NetIncome).FirstOrDefault()) * 2);


            return metric;
        }

        public bool Insert(DashboardDTO entity)
        {
            var ret = false;

            ret = Validate(entity);

            if (ret)
            {
                var newTransaction = ConvertViewModelToTransaction(entity.CreateTransaction);
                _dbEditor.UpdateAccountBalances(newTransaction, "create");
                if (entity.CreateTransaction.UsedCreditCard)
                    _dbEditor.UpdateCreditCardBalances(newTransaction, "create");

                _db.Transactions.Add(newTransaction);
                _db.SaveChanges();
            }

            return ret;
        }

        [ValidateAntiForgeryToken]
        public void Update(DashboardDTO entity)
        {
            Transaction transaction = ConvertViewModelToTransaction(entity.CreateTransaction);

            if (transaction != null)
            {
                _dbEditor.UpdateAccountBalances(transaction, "edit");
                _dbEditor.UpdateCreditCardBalances(transaction, "edit");

                _db.Entry(transaction).State = EntityState.Modified;

                var creditCard = new CreditCard();
                if (transaction.UsedCreditCard)
                {
                    creditCard = entity.CreateTransaction.CreditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                }

                _db.SaveChanges();
            }
        }

        public void DeleteTransaction(int? id)
        {
            Transaction transaction = _db.Transactions.Find(id);
            try
            {
                _db.Transactions.Remove(transaction);
                _dbEditor.UpdateAccountBalances(transaction, "delete");
                _dbEditor.UpdateCreditCardBalances(transaction, "delete");

                if (transaction.UsedCreditCard)
                {
                    var creditCards = _db.CreditCards.ToList();
                    var creditCard = creditCards.FirstOrDefault(c => c.Id == transaction.SelectedCreditCardAccount);
                    _db.Entry(creditCard).State = EntityState.Modified;
                }

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private bool Validate(DashboardDTO entity)
        {
            ValidationErrors.Clear();

            if (entity.CreateTransaction.Amount <= 0)
            {
                ValidationErrors.Add(new
                    KeyValuePair<string, string>("Transaction",
                        "Transaction amount must be greater than 0."));
            }

            return (ValidationErrors.Count == 0);
        }

        private Transaction ConvertViewModelToTransaction(TransactionViewModel transactionViewModel)
        {
            try
            {
                var newTransaction = new Transaction();
                newTransaction.Id = transactionViewModel.Id;
                newTransaction.Date = transactionViewModel.Date;
                newTransaction.Payee = transactionViewModel.Payee;
                newTransaction.Memo = transactionViewModel.Memo;
                newTransaction.Type = transactionViewModel.Type;
                newTransaction.Category = transactionViewModel.Category;
                newTransaction.CreditAccount = _db.Accounts.FirstOrDefault(a => a.Id == transactionViewModel.SelectedCreditAccount);
                newTransaction.CreditAccountId = transactionViewModel.SelectedCreditAccount;
                newTransaction.DebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == transactionViewModel.SelectedDebitAccount);
                newTransaction.DebitAccountId = transactionViewModel.SelectedDebitAccount;
                newTransaction.Amount = transactionViewModel.Amount;
                newTransaction.UsedCreditCard = transactionViewModel.UsedCreditCard;
                newTransaction.SelectedCreditCardAccount = transactionViewModel.SelectedCreditCardAccount;


                return newTransaction;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
