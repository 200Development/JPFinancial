using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface IDashboardViewModel
    {
        List<Account> Accounts { get; set; }
        List<Bill> Bills { get; set; }
        List<Expense> Expenses { get; set; }
        List<Transaction> Transactions { get; set; }
        DashboardMetrics Metrics { get; set; }
        TimeValueOfMoneyMetrics TimePeriodMetrics { get; set; }
    }
}