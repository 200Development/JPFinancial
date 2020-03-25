using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JPFData;
using JPFData.Managers;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFinancial.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly AccountManager _accountManager = new AccountManager();
        private readonly DashboardManager _dashboardManager = new DashboardManager();


        // GET: Dashboard
        public ActionResult Index()
        {
            try
            {
                DashboardViewModel dashboardVM = new DashboardViewModel();
                dashboardVM.Accounts = _accountManager.GetAllAccounts();
                dashboardVM.Metrics = _dashboardManager.RefreshStaticMetrics();
                _accountManager.Update();
                _accountManager.Rebalance();


                return View(dashboardVM);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return View("Error");
            }
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
