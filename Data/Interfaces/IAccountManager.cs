using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface IAccountManager
    {
        List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        bool Create(Account account);
        bool Edit(Account account);
        bool UpdateAccountsFromDashboard(IList<Account> accounts);
        bool Delete(int id);
        List<Account> GetAllAccounts();
        Account GetAccount(int? id);
        Account GetPoolAccount();
        Account GetEmergencyFundAccount();
        AccountMetrics GetMetrics();
        AccountRebalanceReport GetRebalancingAccountsReport();
        void CheckAndCreatePoolAccount();
        void CheckAndCreateEmergencyFund();

        /// <summary>
        /// Updates Account balances in the database.  Uses surplus balances to pay off deficits
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        bool Update();

        bool Rebalance();
    }
}