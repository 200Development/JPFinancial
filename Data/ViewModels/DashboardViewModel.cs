using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            Accounts = new List<Account>();
            Bills = new List<Bill>();
            Expenses = new List<Expense>();
            Transactions = new List<Transaction>();
            Metrics = new DashboardMetrics();
            TimePeriodMetrics = new TimeValueOfMoneyMetrics();
        }
        

        public List<Account> Accounts { get; set; }
        public List<Bill> Bills { get; set; }
        public List<Expense> Expenses { get; set; }
        public List<Transaction> Transactions { get; set; }
        public DashboardMetrics Metrics { get; set; }
        public TimeValueOfMoneyMetrics TimePeriodMetrics { get; set; }
    }
}