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
    /// <summary>
    /// Handles all Account interactions with the database
    /// </summary>
    public class AccountManager
    {
        private readonly ApplicationDbContext _db;
        private readonly Calculations _calc;
        private int _dbTransactions;


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


                _db.SaveChanges();
                entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(entity);
            }
            catch (Exception)
            {
                //ignore
            }


            return entity;
        }

        public Account Details(AccountDTO entity)
        {
            return _db.Accounts.FirstOrDefault(a => a.Id == entity.Account.Id);
        }

        public bool Edit(AccountDTO entity)
        {
            try
            {
                _db.Entry(entity.Account).State = EntityState.Modified;
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

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


        #region Update

        /// <summary>
        /// Updates Account balances in the database.  Uses surplus balances to pay off deficits
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public AccountDTO Update(AccountDTO entity)
        {
            try
            {
                entity.Accounts = _db.Accounts.ToList();
                // Refresh Account surpluses
                if (!UpdateBalanceSurplus()) return entity;

                // pay off Account deficits with pool
                if (!UpdateRequiredBalance()) return entity;

                // update paycheck contributions to be suggested contributions
                if (!UpdatePaycheckContributions()) return entity;


                // Save changes to the database if all Account updates ran successfully
                _db.SaveChanges();
            }
            catch (Exception)
            {
                //ignore
            }


            return entity;
        }

        /// <summary>
        /// Database update if dbSave = true, else EntityState.Modified.
        /// Update the balance surplus for each Account.
        /// Balance surplus = balance - required savings.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateBalanceSurplus(bool dbSave = false)
        {
            // public because runs on startup with dbSave = true
            try
            {
                foreach (var account in _db.Accounts.ToList())
                {
                    account.BalanceSurplus = account.Balance - account.RequiredSavings;
                    _db.Entry(account).State = EntityState.Modified;
                    _dbTransactions += 1;
                }


                if (!dbSave) return true;
                _db.SaveChanges();
                _dbTransactions = 0;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //TODO: research access modifiers 
        /// <summary>
        /// Database update if dbSave = true, else EntityState.Modified.
        /// Update the required savings for each Account.
        /// Required savings = 
        /// </summary>
        public bool UpdateRequiredBalance(bool dbSave = false)
        {
            // public because runs on startup with dbSave = true
            try
            {
                var savingsAccountBalances = new List<KeyValuePair<string, decimal>>();

                foreach (var bill in _db.Bills.ToList())
                {
                    bill.Account = _db.Accounts.FirstOrDefault(a => a.Id == bill.AccountId);
                    if (bill.Account == null) continue;
                    var billTotal = bill.AmountDue;
                    // Next due date
                    var dueDate = bill.DueDate;
                    // How many pay periods to save until next due date
                    var payPeriodsLeft = _calc.PayPeriodsTilDue(dueDate);
                    decimal savePerPaycheck = 0;

                    // Calculate how much to save from each pay period
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
                    // required savings = bill amount due - (how many pay periods before due date * how much to save per pay period)
                    var save = billTotal - payPeriodsLeft * savePerPaycheck;
                    // add kvp (account that bill is credited to, amount to save) 

                    savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }

                // update each account that has a bill credited to it 
                foreach (var account in _db.Accounts.ToList())
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
                    _dbTransactions += 1;
                }
                if (!dbSave) return true;
                _db.SaveChanges();
                _dbTransactions = 0;


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Entity update.
        /// Update the paycheck contributions for each account.
        /// Paycheck contribution = suggested paycheck contribution.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool UpdatePaycheckContributions()
        {
            try
            {
                foreach (var account in _db.Accounts.Where(a => !a.ExcludeFromSurplus)
                    .Where(a => a.SuggestedPaycheckContribution > 0))
                {
                    account.PaycheckContribution = account.SuggestedPaycheckContribution;
                    _db.Entry(account).State = EntityState.Modified;
                    _dbTransactions += 1;
                }


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Rebalance

        public AccountDTO Rebalance(AccountDTO entity)
        {
            try
            {
                if (!PoolSurplus(entity)) return entity;
                if (!RebalanceAccountSavings(entity)) return entity;
                //if (!UpdatePaycheckContributions(entity)) return entity;
                entity.RebalanceReport = new Calculations().GetRebalancingAccountsReport(entity);


                _db.SaveChanges();
            }
            catch (Exception)
            {
                //ignore
            }


            return entity;
        }

        /// <summary>
        /// Entity update.
        /// Move all Account surpluses to the designated pool Account.
        /// Pool Account balance += Accounts' surplus
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Entity update.
        /// Update balance for Accounts with deficits.
        /// While (pool.balance > 0) take deficit amount from pool and add to deficit Account balance.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool RebalanceAccountSavings(AccountDTO entity)
        {
            try
            {
                var poolAccount = entity.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) throw new Exception("Pool account has not been assigned");


                foreach (var account in entity.Accounts)
                {
                    if (poolAccount.Balance <= 0) break;
                    if (account.ExcludeFromSurplus || account.BalanceSurplus >= 0) continue;
                    if (account.BalanceSurplus == null) continue;
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
                        // DB add transaction
                        _db.Entry(newTransaction).State = EntityState.Added;
                        // DB update deficit Account balance
                        _db.Entry(account).State = EntityState.Modified;
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
                        // DB add transaction
                        _db.Entry(newTransaction).State = EntityState.Added;
                        // DB update deficit Account balance
                        _db.Entry(account).State = EntityState.Modified;
                    }
                }
                // DB update pool Account 
                _db.Entry(poolAccount).State = EntityState.Modified;


                //_db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
