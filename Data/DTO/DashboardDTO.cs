using System.Collections.Generic;
using JPFData.Models;
using JPFData.ViewModels;

namespace JPFData.DTO
{
    public class DashboardDTO
    {
        public DashboardDTO()
        {
            Accounts = new List<Account>();
            Transactions = new List<Transaction>();
            CreateTransaction = new CreateTransactionViewModel();
            Transaction = new Transaction();
            StaticFinancialMetrics = new StaticFinancialMetrics();
            TimePeriodMetrics = new TimePeriodFinancialMetrics();
        }

        public List<Account> Accounts { get; set; }
        public List<Transaction> Transactions { get; set; }
        public CreateTransactionViewModel CreateTransaction { get; set; }
        public Transaction Transaction { get; set; }
        public StaticFinancialMetrics StaticFinancialMetrics { get; set; }
        public TimePeriodFinancialMetrics TimePeriodMetrics { get; set; }
    }
}
