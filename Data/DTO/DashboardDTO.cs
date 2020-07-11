using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.DTO
{
    public class DashboardDTO
    {
        public DashboardDTO()
        {
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
            CreditCards = new List<CreditCard>();
            StaticFinancialMetrics = new StaticFinancialMetrics();
            TimePeriodMetrics = new TimeValueOfMoneyMetrics();
        }

        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<CreditCard> CreditCards { get; set; }
        public StaticFinancialMetrics StaticFinancialMetrics { get; set; }
        public TimeValueOfMoneyMetrics TimePeriodMetrics { get; set; }
    }
}
