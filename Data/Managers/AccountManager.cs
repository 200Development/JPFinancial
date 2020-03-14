using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Accounts Table
    /// </summary>
    public class AccountManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;
        private readonly Calculations _calc;


        public AccountManager()
        {
            _db = new ApplicationDbContext();
            _calc = new Calculations();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;

            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public List<Account> GetAllAccounts()
        {
            try
            {
                // should pool account be excluded?
                return _db.Accounts.Where(a => a.UserId == _userId && !a.IsPoolAccount).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public Account GetAccount(int? id)
        {
            try
            {
                return _db.Accounts.Find(id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public Account GetPoolAccount()
        {
            try
            {
                return _db.Accounts.Where(a => a.UserId == _userId).FirstOrDefault(a => a.IsPoolAccount);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(Account account)
        {
            try
            {
                _db.Accounts.Add(account);
                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(Account account)
        {
            try
            {
                account.BalanceSurplus = _calc.UpdateBalanceSurplus(account);

                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var billManager = new BillManager();
                var transactionManager = new TransactionManager();

                var account = _db.Accounts.Find(id);
                if (account.IsPoolAccount)
                    throw new Exception("Cannot delete pool account");

                var bills = billManager.GetAllBills().Where(b => b.AccountId == id);
                var transactions = transactionManager.GetAllTransactions()
                    .Where(t => t.CreditAccountId == id || t.DebitAccountId == id);

                foreach (var bill in bills)
                {
                    billManager.Delete(bill.Id);
                }

                foreach (var transaction in transactions)
                {
                    transactionManager.Delete(transaction.Id);
                }

                _db.Accounts.Remove(account);
                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public AccountMetrics GetMetrics()
        {
            try
            {
                //TODO: Needs Refactoring
                var metrics = new AccountMetrics();
                var accountManager = new AccountManager();
                var billManager = new BillManager();
                var accounts = GetAllAccounts();
                var poolAccount = GetPoolAccount();
                var requiredSavingsDict = _calc.GetRequiredSavingsDict();
                var totalRequiredSavings = 0.0m;

                foreach (var savings in requiredSavingsDict)
                {
                    try
                    {
                        totalRequiredSavings += savings.Value;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                if (!accounts.Any()) return metrics;

                metrics.LargestBalance = accounts.Max(a => a.Balance);
                metrics.SmallestBalance = accounts.Min(a => a.Balance);
                metrics.AverageBalance = accounts.Sum(a => a.Balance) / accounts.Count;
                var incomeTransactions = _db.Transactions.Where(t => t.Type == TransactionTypesEnum.Income);
                var oldestIncomeTransaction = incomeTransactions.OrderBy(t => t.Date).FirstOrDefault();
                var daysAgo = 0;
                if (oldestIncomeTransaction != null)
                    daysAgo = (DateTime.Today - oldestIncomeTransaction.Date).Days;
                var monthsAgo = daysAgo / 30 < 1 ? 1 : daysAgo / 30;


                if (incomeTransactions.Any())
                    metrics.MonthlySurplus = (incomeTransactions.Sum(t => t.Amount) / monthsAgo) - (accounts.Sum(a => a.PaycheckContribution) * 2);
                metrics.LargestSurplus = accounts.Max(a => a.BalanceSurplus);
                metrics.SmallestSurplus = accounts.Min(a => a.BalanceSurplus);
                var surplusAccounts = accounts.Where(a => a.BalanceSurplus > 0).ToList().Count; if (surplusAccounts > 0)
                    metrics.AverageSurplus = accounts.Sum(a => a.BalanceSurplus) / surplusAccounts;
                
                var cashBalance = accounts.Sum(a => a.Balance) + poolAccount.Balance;
                var outstandingExpenses = billManager.GetOutstandingExpenseTotal();

                metrics.CashBalance = cashBalance;
                metrics.AccountingBalance = cashBalance - totalRequiredSavings;

                var totalSurplus = accounts.Sum(a => a.BalanceSurplus) + poolAccount.Balance;
                metrics.SpendableCash = totalSurplus > 0 ? totalSurplus : 0.0m; // Account balance surplus = account balance - balance limit (balance limit is 0, surplus = balance - required savings).  Balance limit allows the account to "fill up" to the limit 
                metrics.OutstandingExpenses = outstandingExpenses;
                metrics.PoolBalance = accountManager.GetPoolAccount().Balance;
                metrics.OutstandingAccountDeficit = accounts.Where(a => a.BalanceSurplus < 0).Sum(a => a.BalanceSurplus);

                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public void CheckAndCreatePoolAccount()
        {
            try
            {
                var accounts = _db.Accounts.Where(a => a.UserId == _userId).ToList();
                if (accounts.Exists(a => a.IsPoolAccount)) return;

                var poolAccount = new Account();
                poolAccount.Name = "Pool";
                poolAccount.Balance = 0.0m;;
                poolAccount.IsPoolAccount = true;
                poolAccount.UserId = _userId;

                _db.Accounts.Add(poolAccount);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public bool Update(List<Account> accounts)
        {
            return _calc.Update(accounts);
        }

        public bool Rebalance(List<Account> accounts)
        {
            return _calc.Rebalance(accounts);
        }
    }
}
