﻿using JPFinancial.Models;
using JPFinancial.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using JPFinancial.Models.Enumerations;

namespace JPFinancial.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();

        // GET: Dashboard
        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel();
            var income = GetMonthlyIncome();

            viewModel.CurrentMonth = DateTime.Today.ToString("MMMM", CultureInfo.CurrentCulture);
            viewModel.OneMonthSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(1), income).ToString("C", CultureInfo.CurrentCulture);
            viewModel.ThreeMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(3), income).ToString("C", CultureInfo.CurrentCulture);
            viewModel.SixMonthsSavings = _calculations.CalculateFv(DateTime.Today.AddMonths(6), income).ToString("C", CultureInfo.CurrentCulture);
            viewModel.OneYearSavings = _calculations.CalculateFv(DateTime.Today.AddYears(1), income).ToString("C", CultureInfo.CurrentCulture);

            var savingsAccountBalances = new Dictionary<string, decimal>();
            var bills = _db.Bills.ToList();
            var accounts = _db.Accounts.ToList();
            var transactions = _db.Transactions.ToList();
            var sortedTransactions = SortTransactions(transactions, 1);
            var firstDayOfMonth = _calculations.GetFirstDayOfMonth(DateTime.Today.Year, DateTime.Today.Month);
            var lastDayOfMonth = _calculations.GetLastDayOfMonth(DateTime.Today);
            var firstPaycheck = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15); //ToDo: make dynamic
            var lastPaycheck = _calculations.GetLastDayOfMonth(DateTime.Today);
            var billsDue = _calculations.GetBillsDue(firstPaycheck, firstDayOfMonth, new DateTime(DateTime.Today.Year, DateTime.Today.Month, lastPaycheck));
            var totalDue = billsDue.Sum(bill => Convert.ToDecimal(bill.AmountDue));
            savingsAccountBalances = _calculations.SavingsReqForBills(bills, savingsAccountBalances);


            _calculations.UpdateAccountGoals(accounts, savingsAccountBalances);


            viewModel.MonthlyExpenses = totalDue.ToString("C", CultureInfo.CurrentCulture);
            viewModel.MonthlyIncome = (Convert.ToDecimal(_db.Salaries.Select(s => s.NetIncome).FirstOrDefault()) * 2).ToString("C", CultureInfo.CurrentCulture);
            viewModel.SavedUp = _calculations.SavingsReqForBills(bills).ToString("C", CultureInfo.CurrentCulture);
            viewModel.TotalDue = totalDue.ToString("C", CultureInfo.CurrentCulture);
            viewModel.Accounts = accounts;

            DateTime lastMonth = DateTime.Today.AddMonths(-1);
            LoanViewModel loanViewModel = new LoanViewModel();
            loanViewModel.ExpenseRatio = _calculations.CalculateExpenseRatio();
            var financialsPerMonth = new List<Dictionary<DateTime, LoanViewModel>>();
            var financialsDictionary = new Dictionary<DateTime, LoanViewModel> { { lastMonth, loanViewModel } };
            financialsPerMonth.Add(financialsDictionary);

            viewModel.LoanViewModelByMonth = financialsPerMonth;

           

            return View(viewModel);
        }

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

        public Chart AccountsGraph()
        {
            var accounts = _db.Accounts.ToList();

            var accountNames = accounts.Select(a => a.Name).ToArray();
            var accountBalances = accounts.Select(a => a.Balance).ToArray();

            var accountGraph = new Chart(900, 500)
                .AddTitle("Accounts")
                .AddSeries(
                    name: "Account",
                    xValue: accountNames,
                    yValues: accountBalances)
                .Write();

            accountGraph.Save("~/Content/chart" + "jpeg");
            return accountGraph;
        }

        private SortedTransactions SortTransactions(IEnumerable<Transaction> transactions, int yearsBack)
        {
            var date = DateTime.Today;
            var year = date.Year;
            var month = date.Month;

            var howFarBack = date.AddYears(-yearsBack);
            var previousYear = new DateTime(howFarBack.Year, 1, 1);
            var sortedTransactions = new SortedTransactions();

            var balancesByMonth = new Dictionary<string, decimal>();
            var balancesByQuarter = new Dictionary<string, decimal>();
            var balancesByYear = new Dictionary<string, decimal>();
            var balancesByType = new Dictionary<string, decimal>();
            var balancesByCategory = new Dictionary<string, decimal>();
            var previousYearBalancesByMonth = new Dictionary<string, decimal>();
            var previousYearBalancesByQuarter = new Dictionary<string, decimal>();
            var previousYearBalancesByYear = new Dictionary<string, decimal>();
            var previousYearBalancesByType = new Dictionary<string, decimal>();
            var previousYearBalancesByCategory = new Dictionary<string, decimal>();

            var yearBackTransactions = transactions.Where(t => t.Date > howFarBack);
            var lastYearTransactions = transactions.Where(t => t.Date >= previousYear && t.Date <= new DateTime(previousYear.Year, 12, 31));

            foreach (var transaction in yearBackTransactions)
            {
                var transactMonth = transaction.Date.Month;
                var transactYear = transaction.Date.Year;
                var monthKey = string.Concat(transactMonth.ToString(), transactYear);
                if (balancesByMonth.ContainsKey(monthKey))
                {
                    var currentBalance = balancesByMonth[$"{monthKey}"];
                    var newBalance = Convert.ToDecimal(transaction.Amount + currentBalance);
                    balancesByMonth[monthKey] = newBalance;
                }
                else
                {
                    balancesByMonth.Add(monthKey, Convert.ToDecimal(transaction.Amount));
                }
            }

            foreach (var transaction in lastYearTransactions)
            {
                var transactMonth = transaction.Date.Month;
                var transactYear = transaction.Date.Year;
                var monthKey = string.Concat(transactMonth.ToString(), transactYear);
                if (previousYearBalancesByMonth.ContainsKey(monthKey))
                {
                    var currentBalance = previousYearBalancesByMonth[$"{monthKey}"];
                    var newBalance = Convert.ToDecimal(transaction.Amount + currentBalance);
                    previousYearBalancesByMonth[monthKey] = newBalance;

                    if (transactMonth == 1 || transactMonth == 2 || transactMonth == 3)
                        previousYearBalancesByQuarter[$"firstQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 4 || transactMonth == 5 || transactMonth == 6)
                        previousYearBalancesByQuarter[$"secondQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 7 || transactMonth == 8 || transactMonth == 9)
                        previousYearBalancesByQuarter[$"thirdQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 10 || transactMonth == 11 || transactMonth == 12)
                        previousYearBalancesByQuarter[$"forthQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                }
                else
                {
                    previousYearBalancesByMonth.Add(monthKey, Convert.ToDecimal(transaction.Amount));

                    if (transactMonth == 1 || transactMonth == 2 || transactMonth == 3)
                        previousYearBalancesByQuarter[$"firstQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 4 || transactMonth == 5 || transactMonth == 6)
                        previousYearBalancesByQuarter[$"secondQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 7 || transactMonth == 8 || transactMonth == 9)
                        previousYearBalancesByQuarter[$"thirdQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                    if (transactMonth == 10 || transactMonth == 11 || transactMonth == 12)
                        previousYearBalancesByQuarter[$"forthQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                }
            }

            sortedTransactions.BalanceByMonth = balancesByMonth;
            sortedTransactions.LastYearBalanceByMonth = previousYearBalancesByMonth;
            sortedTransactions.BalanceByQuarter = balancesByQuarter;
            sortedTransactions.LastYearBalanceByQuarter = previousYearBalancesByQuarter;
            sortedTransactions.BalanceByYear = balancesByYear;
            sortedTransactions.LastYearBalanceByYear = previousYearBalancesByYear;
            sortedTransactions.BalanceByType = balancesByType;
            sortedTransactions.LastYearBalanceByType = previousYearBalancesByType;
            sortedTransactions.BalanceByCategory = balancesByCategory;
            sortedTransactions.LastYearBalanceByCategory = previousYearBalancesByCategory;

            return sortedTransactions;
        }


        public ActionResult GetFutureValue(DashboardViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);
            if (model.SelectedFVType.Equals("futureValue"))
            {
                var fv = model.FutureAmount;
                model.FutureDate = _calculations.CalculateFvDate(Convert.ToDecimal(fv), model.NetIncome);
            }
            else if (model.SelectedFVType.Equals("futureDate"))
            {
                model.FutureAmount = _calculations.CalculateFv(Convert.ToDateTime(model.FutureDate), model.NetIncome);
            }
            return View("Index", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class SortedTransactions
    {
        public Dictionary<string, decimal> BalanceByMonth { get; set; }
        public Dictionary<string, decimal> LastYearBalanceByMonth { get; set; }
        public Dictionary<string, decimal> BalanceByQuarter { get; set; }
        public Dictionary<string, decimal> LastYearBalanceByQuarter { get; set; }
        public Dictionary<string, decimal> BalanceByYear { get; set; }
        public Dictionary<string, decimal> LastYearBalanceByYear { get; set; }
        public Dictionary<string, decimal> BalanceByType { get; set; }
        public Dictionary<string, decimal> LastYearBalanceByType { get; set; }
        public Dictionary<string, decimal> BalanceByCategory { get; set; }
        public Dictionary<string, decimal> LastYearBalanceByCategory { get; set; }
    }
}
