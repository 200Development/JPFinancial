using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class AccountDTO
    {
        public AccountDTO()
        {
            Account  = new Account();
            Accounts = new List<Account>();
            Metrics = new AccountMetrics();
            RebalanceReport = new AccountRebalanceReport();
        }

        public Account Account { get; set; }
        public List<Account> Accounts { get; set; }
        public AccountMetrics Metrics { get; set; }
        public AccountRebalanceReport RebalanceReport { get; set; }
    }
}
