using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Models;

namespace JPFData.Managers
{
    public class AccountManager
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly Calculations _calculations = new Calculations();
        private readonly DatabaseEditor _dbEditor = new DatabaseEditor();


        public AccountManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public AccountDTO Get()
        {
            return Get(new AccountDTO());
        }

        public AccountDTO Get(AccountDTO entity)
        {
            AccountDTO ret = new AccountDTO();

            try
            {
                ret.Accounts = _db.Accounts.ToList();
                ret.AccountsMetrics = RefreshAccountMetrics(ret);
                ret.Accounts = UpdateSavingsPercentage(ret.Accounts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
       

            return ret;
        }

        private List<Account> UpdateSavingsPercentage(List<Account> accounts)
        {
            if (accounts != null && accounts.Count > 0)
            {
                var totalSavings = accounts.Sum(a => a.RequiredSavings) ?? decimal.Zero; ;
                foreach (Account account in accounts)
                {
                    var accountSavings = account.RequiredSavings ?? decimal.Zero;
                    var savingsPercentage = accountSavings / totalSavings;
                    account.PercentageOfSavings = savingsPercentage * 100;
                }
            }

            return accounts;
        }

        private AccountsMetrics RefreshAccountMetrics(AccountDTO dto)
        {
            AccountsMetrics metrics = new AccountsMetrics();
            var income = _calculations.GetMonthlyIncome();

            metrics.LargestBalance = dto.Accounts.Max(a => a.Balance);
            metrics.SmallestBalance = dto.Accounts.Min(a => a.Balance);
            metrics.AverageBalance = dto.Accounts.Sum(a => a.Balance) / dto.Accounts.Count;

            metrics.LargestSurplus = dto.Accounts.Max(a => a.BalanceSurplus ?? 0m);
            metrics.SmallestSurplus = dto.Accounts.Min(a => a.BalanceSurplus ?? 0m);
            metrics.AverageSurplus = dto.Accounts.Sum(a => a.BalanceSurplus ?? 0) / dto.Accounts
                                         .Where(a => a.BalanceSurplus != null && a.BalanceSurplus != 0m).ToList().Count;
            

            return metrics;
        }
    }
}
