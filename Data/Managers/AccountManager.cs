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
            try
            {
                entity.Accounts = _db.Accounts.ToList();
                entity.AccountsMetrics = RefreshAccountMetrics(entity);
                entity.Accounts = UpdateSavingsPercentage(entity.Accounts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return entity;
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
                    account.PercentageOfSavings = decimal.Round(savingsPercentage * 100, 2, MidpointRounding.AwayFromZero);
                }
            }

            return accounts;
        }

        private AccountsMetrics RefreshAccountMetrics(AccountDTO dto)
        {
            AccountsMetrics metrics = new AccountsMetrics();

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
