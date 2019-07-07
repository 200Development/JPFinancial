using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

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
                Logger.Instance.DataFlow($"Return list of Accounts");
                return _db.Accounts.Where(a => a.UserId == _userId).ToList();

                //accountVM.Metrics = RefreshAccountMetrics(accountVM);
                //accountVM.RebalanceReport = _calc.GetRebalancingAccountsReport(accountVM);
                //Logger.Instance.DataFlow($"Get rebalancing accounts report");
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
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");
                return _db.Accounts.Find(id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(AccountViewModel accountVM)
        {
            try
            {
                _db.Accounts.Add(accountVM.Account);
                Logger.Instance.DataFlow($"New Account added to data context");

                _db.SaveChanges();
                Logger.Instance.DataFlow($"Saved changes to DB");


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(AccountViewModel accountVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Save changes to DB");
                accountVM.Account.BalanceSurplus = _calc.UpdateAccountSurplus(accountVM.Account);

                _db.Entry(accountVM.Account).State = EntityState.Modified;
                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Delete(int accountId)
        {
            try
            {
                Logger.Instance.DataFlow($"Pull Account with Id of {accountId} from DB");

                Account account = _db.Accounts.Find(accountId);
                if (account.IsPoolAccount)
                    throw new NotImplementedException("Cannot delete pool account");

                //Add the account balance to the pool account.  Should this be default, optional, or never?
                //Account poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                //if (poolAccount != null) poolAccount.Balance += account.Balance;
                //Logger.Instance.Info($"Transfers ${account.Balance} to pool account");

                _db.Accounts.Remove(account);
                //_db.Entry(poolAccount).State = EntityState.Modified;
                Logger.Instance.Info($"Account with id of {accountId} has been flagged for removal from DB");


                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public AccountMetrics GetAccountMetrics()
        {
            try
            {
                Logger.Instance.DataFlow($"Refresh Account metrics");
                AccountMetrics metrics = new AccountMetrics();
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
                metrics.SpendableCash = accounts.Sum(a => a.BalanceSurplus); // An Account balance surplus is any sum over the required savings and balance limit.  Balance limit allows the account to "fill up" to the limit 
                metrics.OutstandingExpenses = outstandingExpenses;

                Logger.Instance.DataFlow($"Return Account metrics");
                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public IList<Account> GetPaycheckContributions()
        {
            try
            {
                return _db.Accounts.Where(a => a.UserId == Global.Instance.User.Id).Where(a => !a.ExcludeFromSurplus).Where(a => a.SuggestedPaycheckContribution > 0).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new List<Account>();
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
