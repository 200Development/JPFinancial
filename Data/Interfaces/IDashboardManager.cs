using System.Collections.Generic;
using JPFData.Metrics;

namespace JPFData.Interfaces
{
    public interface IDashboardManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        DashboardMetrics RefreshStaticMetrics();
        decimal GetDisposableIncome();
        decimal GetSavingsRate();
        decimal GetMinimumMonthlyExpenses();
        Dictionary<string, decimal> GetCashFlowByMonth();
        decimal GetEmergencyFundRatio();
        decimal GetDueBeforeNextPayPeriod();
        decimal GetCashBalance();
        string ConvertMonthIntToString(int month);
    }
}