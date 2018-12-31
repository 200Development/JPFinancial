using System.Collections.Generic;
using JPFData.Models;

namespace JPFData.DTO
{
    public class AccountDTO
    {
        public AccountDTO()
        {

        }

        public List<Account> Accounts { get; set; }
        public AccountsMetrics AccountsMetrics { get; set; }
    }
}
