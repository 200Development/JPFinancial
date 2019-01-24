using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.Managers
{
    public class AccountManager
    {
        private readonly ApplicationDbContext _db;
        private readonly Calculations _calc;


        public AccountManager()
        {
            _db = new ApplicationDbContext();
            _calc = new Calculations();
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
                entity.Metrics = RefreshAccountMetrics(entity);
                entity.Accounts = UpdateSavingsPercentage(entity.Accounts);

                // TODO: Change to manually run rebalancing
                if (!PoolSurplus(entity)) return entity;
                if (!RebalanceAccountSavings(entity)) return entity;
                if (!RebalancePaycheckContributions(entity)) return entity;
                if (!RebalanceAccountSurplus(entity)) return entity;

                _db.SaveChanges();
                entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(entity);
            }
            catch (Exception)
            {
                //ignore
            }


            return entity;
        }



        #region DTO Model Updating

        private List<Account> UpdateSavingsPercentage(List<Account> accounts)
        {
            if (accounts == null || accounts.Count <= 0) return accounts;


            var totalSavings = accounts.Sum(a => a.RequiredSavings) ?? decimal.Zero; ;
            foreach (Account account in accounts)
            {
                var accountSavings = account.RequiredSavings ?? decimal.Zero;
                var savingsPercentage = accountSavings / totalSavings;
                account.PercentageOfSavings = decimal.Round(savingsPercentage * 100, 2, MidpointRounding.AwayFromZero);
            }

            return accounts;
        }

        private AccountMetrics RefreshAccountMetrics(AccountDTO entity)
        {
            AccountMetrics metrics = new AccountMetrics();

            metrics.LargestBalance = entity.Accounts.Max(a => a.Balance);
            metrics.SmallestBalance = entity.Accounts.Min(a => a.Balance);
            metrics.AverageBalance = entity.Accounts.Sum(a => a.Balance) / entity.Accounts.Count;

            metrics.LargestSurplus = entity.Accounts.Max(a => a.BalanceSurplus ?? 0m);
            metrics.SmallestSurplus = entity.Accounts.Min(a => a.BalanceSurplus ?? 0m);
            metrics.AverageSurplus = entity.Accounts.Sum(a => a.BalanceSurplus ?? 0m) / entity.Accounts
                                         .Where(a => a.BalanceSurplus != null && a.BalanceSurplus != 0m).ToList().Count;
            metrics.TotalBalance = entity.Accounts.Sum(a => a.Balance);


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
            catch (Exception)
            {
                //ignore
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {
                return false;
            }
        }

        //TODO: research access modifiers 
        public void UpdateRequiredBalance()
        {
            try
            {
                var accounts = _db.Accounts.ToList();
                var bills = _db.Bills.ToList();
                var savingsAccountBalances = new List<KeyValuePair<string, decimal>>();

                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    var dueDate = bill.DueDate;
                    var payPeriodsLeft = _calc.PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Annually:
                            savePerPaycheck = billTotal / 24;
                            break;
                        case FrequencyEnum.SemiAnnually:
                            savePerPaycheck = billTotal / 12;
                            break;
                        case FrequencyEnum.Quarterly:
                            savePerPaycheck = billTotal / 6;
                            break;
                        case FrequencyEnum.SemiMonthly:
                            savePerPaycheck = billTotal / 4;
                            break;
                        case FrequencyEnum.Monthly:
                            savePerPaycheck = billTotal / 2;
                            break;
                        case FrequencyEnum.BiWeekly:
                            savePerPaycheck = billTotal;
                            break;
                        case FrequencyEnum.Weekly:
                            savePerPaycheck = billTotal * 2;
                            break;
                        default:
                            savePerPaycheck = billTotal / 2;
                            break;
                    }
                    var save = billTotal - payPeriodsLeft * savePerPaycheck;
                    savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }


                foreach (var account in accounts)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != account.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }
                    if (!valuesFound) continue;
                    account.RequiredSavings = totalSavings;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void UpdateRequiredBalanceSurplus()
        {
            try
            {
                var accounts = _db.Accounts.ToList();

                foreach (var account in accounts)
                {
                    var acctBalance = account.Balance;
                    var reqbalance = account.RequiredSavings;
                    account.BalanceSurplus = acctBalance - reqbalance;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        #endregion
    }
}
