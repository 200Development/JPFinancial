using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Models;
using JPFData.ViewModels;


namespace JPFData.Managers
{
    public class DashboardManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();


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

            ret.Transactions = _db.Transactions.ToList();
            ret.StaticFinancialMetrics = RefreshFinancialMetrics(ret);
            ret.TimePeriodMetrics = RefreshTimePeriodMetrics(ret);
            return ret;
        }

        private StaticFinancialMetrics RefreshFinancialMetrics(DashboardDTO entity)
        {
            StaticFinancialMetrics metrics = new StaticFinancialMetrics();

            var firstDayOfMonth = _calculations.GetFirstDayOfMonth(DateTime.Today.Year, DateTime.Today.Month);
            var firstPaycheck = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15); //ToDo: make dynamic
            var lastPaycheck = _calculations.GetLastDayOfMonth(DateTime.Today);
            var income = GetMonthlyIncome();
            var bills = _db.Bills.ToList();
            var loans = _db.Loans.ToList();

            var monthlyLoanInterest = 0.0m;
            var dailyLoanInterest = 0.0m;
            foreach (var loan in loans)
            {
                monthlyLoanInterest += _calculations.CalculateMonthlyInterestCost(loan);
                dailyLoanInterest += _calculations.CalculateDailyInterestCost(loan);
            }

            //TODO: difference between totalDue and billsDue
            var billsDue = _calculations.GetBillsDue(firstPaycheck, firstDayOfMonth, new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastPaycheck));
            var totalDue = billsDue.Sum(bill => Convert.ToDecimal(bill.AmountDue));

            metrics.MandatoryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => b.IsMandatory).Sum(b => b.AmountDue);
            metrics.DiscretionaryExpenses = bills.Where(b => b.DueDate.Month == DateTime.Today.Month).Where(b => !b.IsMandatory).Sum(b => b.AmountDue);
            metrics.CostliestExpenseAmount = entity.Transactions.Where(t => t.Date.Month == DateTime.Today.AddMonths(-1).Month)
                .OrderByDescending(t => t.Amount)
                .Select(t => t.Amount)
                .Take(1)
                .FirstOrDefault();
            metrics.CostliestCategory = entity.Transactions.Where(t => t.Date.Month == DateTime.Today.AddMonths(-1).Month)
                .OrderByDescending(t => t.Amount)
                .Select(t => t.Category)
                .Take(1)
                .FirstOrDefault();

            metrics.CostliestExpensePercentage = metrics.CostliestExpenseAmount / income;
            metrics.LoanInterestPercentOfIncome = metrics.MonthlyLoanInterest / income;
            metrics.MonthlyLoanInterest = monthlyLoanInterest;
            metrics.DailyLoanInterestPercentage = (dailyLoanInterest / income);
            metrics.DailyLoanInterest = dailyLoanInterest;


            return metrics;
        }

        private TimePeriodFinancialMetrics RefreshTimePeriodMetrics(DashboardDTO entity)
        {
            TimePeriodFinancialMetrics metric = new TimePeriodFinancialMetrics();

            metric.OneMonthSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(1), entity.StaticFinancialMetrics.NetIncome);
            metric.ThreeMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(3), entity.StaticFinancialMetrics.NetIncome);
            metric.SixMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(6), entity.StaticFinancialMetrics.NetIncome);
            metric.OneYearSavings = _calculations.CalculateFv(DateTime.Today.AddYears(1), entity.StaticFinancialMetrics.NetIncome);
            metric.MonthlyExpenses = entity.StaticFinancialMetrics.TotalDue;
            metric.MonthlyIncome = (Convert.ToDecimal(_db.Salaries.Select(s => s.NetIncome).FirstOrDefault()) * 2);
           

            return metric;
        }

        //TODO: Move to Calculations Class
        private decimal GetMonthlyIncome()
        {
            try
            {
                var incomePerPayperiod = Convert.ToDecimal(_db.Salaries.Sum(s => s.NetIncome));
                var paymentFrequency = _db.Salaries.Select(s => s.PayFrequency).FirstOrDefault();
                var monthlyIncome = 0.00m;

                switch (paymentFrequency)
                {
                    case FrequencyEnum.Weekly:
                        monthlyIncome = incomePerPayperiod * 4;
                        break;
                    case FrequencyEnum.SemiMonthly:
                        monthlyIncome = incomePerPayperiod * 2;
                        break;
                    case FrequencyEnum.Monthly:
                        monthlyIncome = incomePerPayperiod;
                        break;
                    default:
                        monthlyIncome = incomePerPayperiod * 2;
                        break;
                }

                return monthlyIncome;
            }
            catch (Exception e)
            {
                return 0.0m;
            }
        }

        public bool Insert(DashboardDTO entity)
        {
            var ret = false;

            ret = Validate(entity);

            if (ret)
            {
                var newTransaction = ConvertViewModelToTransaction(entity.CreateTransaction);
                UpdateAccountBalances(newTransaction, "create");
                if (entity.CreateTransaction.UsedCreditCard)
                    UpdateCreditCard(newTransaction, "create");

                _db.Transactions.Add(newTransaction);
                _db.SaveChanges();
            }

            return ret;
        }

        public void Update(DashboardDTO entity)
        {
            throw new NotImplementedException();
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

        private Transaction ConvertViewModelToTransaction(CreateTransactionViewModel transactionViewModel)
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
                newTransaction.DebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == transactionViewModel.SelectedDebitAccount);
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
        
        private void UpdateAccountBalances(Transaction transaction, string type)
        {
            if (type == "create")
            {
                var newTransaction = new Transaction();
                newTransaction.Date = transaction.Date;
                newTransaction.Payee = transaction.Payee;
                newTransaction.Category = transaction.Category;
                newTransaction.Memo = transaction.Memo;
                newTransaction.Type = transaction.Type;
                newTransaction.DebitAccount = transaction.DebitAccount;
                newTransaction.CreditAccount = transaction.CreditAccount;
                newTransaction.Amount = transaction.Amount;

                if (transaction.DebitAccount != null)
                {
                    transaction.DebitAccount.Balance += transaction.Amount;
                    _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                }

                if (transaction.CreditAccount != null)
                {
                    transaction.CreditAccount.Balance -= transaction.Amount;
                    _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                }
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditAccount =
                    _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.CreditAccountId);
                var originalDebitAccount = _db.Accounts.FirstOrDefault(a => a.Id == originalTransaction.DebitAccountId);
                var originalAmount = originalTransaction.Amount;

                // Reassign the Debit/Credit Account Id's to Transaction Model
                transaction.CreditAccountId = originalTransaction.CreditAccountId;
                transaction.DebitAccountId = originalTransaction.DebitAccountId;

                if (type == "delete")
                {
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance -= transaction.Amount;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance += transaction.Amount;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (originalDebitAccount != null)
                    {
                        originalDebitAccount.Balance += amountDifference;
                        _db.Entry(originalDebitAccount).State = EntityState.Modified;
                    }

                    if (originalCreditAccount != null)
                    {
                        originalCreditAccount.Balance -= amountDifference;
                        _db.Entry(originalCreditAccount).State = EntityState.Modified;
                    }
                }
            }
        }

        private void UpdateCreditCard(Transaction transaction, string type)
        {
            var creditCardId = transaction?.SelectedCreditCardAccount;
            if (creditCardId == null) return;
            var creditCards = _db.CreditCards.ToList();
            var creditCard = creditCards.FirstOrDefault(c => c.Id == creditCardId);

            if (type == "create")
            {
                if (creditCard != null) creditCard.Balance += transaction.Amount;
                _db.Entry(creditCard).State = EntityState.Modified;
            }
            else if (type == "delete" || type == "edit")
            {
                var originalTransaction = _db.Transactions
                    .AsNoTracking()
                    .Where(t => t.Id == transaction.Id)
                    .Cast<Transaction>()
                    .FirstOrDefault();
                if (originalTransaction == null) return;
                var originalCreditCard = _db.CreditCards.FirstOrDefault(a => a.Id == originalTransaction.SelectedCreditCardAccount);
                var originalAmount = originalTransaction.Amount;

                // Reassign the credit card Id to Transaction Model
                transaction.SelectedCreditCardAccount = originalTransaction.SelectedCreditCardAccount;

                if (type == "delete")
                {
                    if (originalCreditCard == null) return;
                    originalCreditCard.Balance -= transaction.Amount;
                    _db.Entry(originalCreditCard).State = EntityState.Modified;
                }
                else if (type == "edit")
                {
                    var amountDifference = transaction.Amount - originalAmount;
                    if (creditCard == null) return;
                    creditCard.Balance += amountDifference;
                    _db.Entry(creditCard).State = EntityState.Modified;
                }
            }
        }
    }
}
