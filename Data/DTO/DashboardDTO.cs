using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;
//using JPFData.ViewModels;

namespace JPFData.DTO
{
    public class DashboardDTO
    {
        public DashboardDTO()
        {
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
            CreditCards = new List<CreditCard>();
            //CreateTransaction = new TransactionViewModel();
            //Transaction = new Transaction();
            StaticFinancialMetrics = new StaticFinancialMetrics();
            TimePeriodMetrics = new TimePeriodFinancialMetrics();
        }

        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<CreditCard> CreditCards { get; set; }
        //public TransactionViewModel CreateTransaction { get; set; }
        //public Transaction Transaction { get; set; }
        public StaticFinancialMetrics StaticFinancialMetrics { get; set; }
        public TimePeriodFinancialMetrics TimePeriodMetrics { get; set; }
    }
}
