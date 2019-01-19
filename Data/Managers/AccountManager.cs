using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
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

                // TODO: Change to manually run rebalancing
                if (!PoolSurplus(entity)) return entity;
                if (!RebalanceAccountSavings(entity)) return entity;
                if (!RebalancePaycheckContributions(entity)) return entity;
                if (!RebalanceAccountSurplus(entity)) return entity;

                _db.SaveChanges();
                entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(entity);
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

        public AccountDTO Rebalance(AccountDTO entity)
        {
            try
            {
                if (!PoolSurplus(entity)) return entity;
                if (!RebalanceAccountSavings(entity)) return entity;
                if (!RebalancePaycheckContributions(entity)) return entity;
                entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(entity);

                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


            return entity;
        }

        private bool PoolSurplus(AccountDTO entity)
        {
            try
            {
                var poolAccount = entity.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) throw new Exception("Pool account has not been assigned");


                foreach (var account in entity.Accounts)
                {
                    var surplus = account.Balance - account.RequiredSavings;
                    if (account.ExcludeFromSurplus || !(surplus > 0)) continue;

                    account.Balance -= (decimal)surplus;
                    poolAccount.Balance += (decimal)surplus;

                    var newTransaction = new Transaction();
                    newTransaction.Date = DateTime.Today;
                    newTransaction.Payee = $"Transfer to {poolAccount.Name}";
                    newTransaction.Category = CategoriesEnum.Rebalance;
                    newTransaction.Memo = "Pool Account Surplus's";
                    newTransaction.Type = TransactionTypesEnum.Transfer;
                    newTransaction.DebitAccount = poolAccount;
                    newTransaction.CreditAccount = account;
                    newTransaction.Amount = (decimal)surplus;
                    _db.Entry(newTransaction).State = EntityState.Added;
                }


                //_db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool RebalanceAccountSavings(AccountDTO entity)
        {
            try
            {
                var poolAccount = entity.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) throw new Exception("Pool account has not been assigned");


                foreach (var account in entity.Accounts)
                {
                    if (poolAccount.Balance <= 0) break;
                    if (account.ExcludeFromSurplus || !(account.BalanceSurplus < 0)) continue;
                    var deficit = (decimal)account.BalanceSurplus * -1;


                    // If pool account doesn't have enough to cover the full deficit, use what is left
                    if (poolAccount.Balance < deficit)
                    {
                        var balance = poolAccount.Balance;
                        account.Balance += balance;
                        poolAccount.Balance -= balance;

                        var newTransaction = new Transaction();
                        newTransaction.Date = DateTime.Today;
                        newTransaction.Payee = $"Transfer to {account.Name}";
                        newTransaction.Category = CategoriesEnum.Rebalance;
                        newTransaction.Memo = "Cover Deficit";
                        newTransaction.Type = TransactionTypesEnum.Transfer;
                        newTransaction.DebitAccount = account;
                        newTransaction.CreditAccount = poolAccount;
                        newTransaction.Amount = balance;
                        _db.Entry(newTransaction).State = EntityState.Added;
                    }
                    else // Make account whole
                    {
                        account.Balance += deficit;
                        poolAccount.Balance -= deficit;

                        var newTransaction = new Transaction();
                        newTransaction.Date = DateTime.Today;
                        newTransaction.Payee = $"Transfer to {account.Name}";
                        newTransaction.Category = CategoriesEnum.Rebalance;
                        newTransaction.Memo = "Cover Deficit";
                        newTransaction.Type = TransactionTypesEnum.Transfer;
                        newTransaction.DebitAccount = account;
                        newTransaction.CreditAccount = poolAccount;
                        newTransaction.Amount = deficit;
                        _db.Entry(newTransaction).State = EntityState.Added;
                    }
                }


                //_db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool RebalancePaycheckContributions(AccountDTO entity)
        {
            try
            {
                foreach (var account in entity.Accounts.Where(a => !a.ExcludeFromSurplus)
                    .Where(a => a.SuggestedPaycheckContribution > 0))
                {
                    account.PaycheckContribution = account.SuggestedPaycheckContribution;
                }


                //_db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool RebalanceAccountSurplus(AccountDTO entity)
        {
            try
            {
                foreach (var account in entity.Accounts)
                {
                    account.BalanceSurplus = account.Balance - account.RequiredSavings;
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
