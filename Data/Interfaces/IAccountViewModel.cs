using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using PagedList;

namespace JPFData.Interfaces
{
    public interface IAccountViewModel
    {
        Account Account { get; set; }
        List<Account> Accounts { get; set; }
        IPagedList<Account> PagedAccounts { get; set; }
        AccountMetrics Metrics { get; set; }
        AccountRebalanceReport RebalanceReport { get; set; }
    }
}