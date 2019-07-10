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
    /// Handles all Account interactions with the database
    /// </summary>
    public class AccountManager
    {
        /*
       STRUCTURE
       private properties
       constructors
       public properties
       public methods
       private methods
       */
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
                Account account = _db.Accounts.Find(id);
                if (account.IsPoolAccount)
                    throw new Exception("Cannot delete pool account");

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
                AccountMetrics metrics = new AccountMetrics();
                AccountManager accountManager = new AccountManager();
                BillManager billManager = new BillManager();
                List<Account> accounts = GetAllAccounts();

                if (accounts.Count < 1) return metrics;

                metrics.LargestBalance = accounts.Max(a => a.Balance);
                metrics.SmallestBalance = accounts.Min(a => a.Balance);
                if (accounts.Count > 0)
                    metrics.AverageBalance = accounts.Sum(a => a.Balance) / accounts.Count;
                else
                    metrics.AverageBalance = 0;
                var incomeTransactions = _db.Transactions.Where(t => t.Type == TransactionTypesEnum.Income);
                var oldestIncomeTransaction = incomeTransactions.OrderBy(t => t.Date).FirstOrDefault();
                var daysAgo = 0;
                if (oldestIncomeTransaction != null)
                    daysAgo = (DateTime.Today - oldestIncomeTransaction.Date).Days;
                var monthsAgo = daysAgo / 30 < 1 ? 1 : daysAgo / 30;


                metrics.MonthlySurplus = (incomeTransactions.Sum(t => t.Amount) / monthsAgo) - (accounts.Sum(a => a.PaycheckContribution) * 2);
                metrics.LargestSurplus = accounts.Max(a => a.BalanceSurplus);
                metrics.SmallestSurplus = accounts.Min(a => a.BalanceSurplus);
                var surplusAccounts = accounts.Where(a => a.BalanceSurplus > 0).ToList().Count; if (surplusAccounts > 0)
                    metrics.AverageSurplus = accounts.Sum(a => a.BalanceSurplus) / surplusAccounts;

                var cashBalance = accounts.Sum(a => a.Balance);
                var outstandingExpenses = billManager.GetOutstandingExpenseTotal();

                metrics.CashBalance = cashBalance;
                metrics.AccountingBalance = cashBalance - outstandingExpenses;
                var sumOfAccountBalances = accounts.Sum(a => a.BalanceSurplus);
                metrics.SpendableCash = sumOfAccountBalances > 0 ? sumOfAccountBalances : 0.0m; // An Account balance surplus is any sum over the required savings and balance limit.  Balance limit allows the account to "fill up" to the limit 
                metrics.OutstandingExpenses = outstandingExpenses;
                metrics.PoolBalance = accountManager.GetPoolAccount().Balance;

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
                poolAccount.Balance = decimal.Zero;
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

        public bool Update()
        {
            return _calc.Update();
        }

        public bool Rebalance()
        {
            return _calc.Rebalance();
        }
    }
}
