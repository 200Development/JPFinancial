using System.Collections.Generic;
using JPFData.Models;

namespace JPFData.DTO
{
    public class AccountDTO
    {
        public AccountDTO()
        {
            Accounts = new List<Account>();
            AccountsMetrics = new AccountsMetrics();
            RebalanceReport = new AccountRebalanceReport();
        }

        public List<Account> Accounts { get; set; }
        public AccountsMetrics AccountsMetrics { get; set; }
        public AccountRebalanceReport RebalanceReport { get; set; }
    }
}
