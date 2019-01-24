using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using JPFData;
using JPFData.Enumerations;
using JPFData.Models;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        // GET: Dashboard
        public ActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel();

            vm.HandleRequest();

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(DashboardViewModel vm)
        {
            vm.IsValid = ModelState.IsValid;
            vm.HandleRequest();

            if (vm.IsValid)
            {
                // NOTE: Must clear the model state in order to bind the @Html helpers to the new model values
                ModelState.Clear();
            }
            else
            {
                foreach (KeyValuePair<string, string> item in vm.ValidationErrors)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }
            }

            return View(vm);
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
            catch (Exception)
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

                    switch (transactMonth)
                    {
                        case 1:
                        case 2:
                        case 3:
                            previousYearBalancesByQuarter[$"firstQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            previousYearBalancesByQuarter[$"secondQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                            break;
                        case 7:
                        case 8:
                        case 9:
                            previousYearBalancesByQuarter[$"thirdQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                            break;
                        case 10:
                        case 11:
                        case 12:
                            previousYearBalancesByQuarter[$"forthQuarter{transactYear}"] += Convert.ToDecimal(transaction.Amount);
                            break;
                    }
                }
                else
                {
                    previousYearBalancesByMonth.Add(monthKey, Convert.ToDecimal(transaction.Amount));

                    switch (transactMonth)
                    {
                        case 1:
                        case 2:
                        case 3:
                            previousYearBalancesByQuarter[$"firstQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            previousYearBalancesByQuarter[$"secondQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                            break;
                        case 7:
                        case 8:
                        case 9:
                            previousYearBalancesByQuarter[$"thirdQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                            break;
                        case 10:
                        case 11:
                        case 12:
                            previousYearBalancesByQuarter[$"forthQuarter{transactYear}"] = Convert.ToDecimal(transaction.Amount);
                            break;
                    }
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
