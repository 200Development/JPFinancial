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


        public AccountManager()
        {
            _db = new ApplicationDbContext();
            _calc = new Calculations();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public AccountDTO Get(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Get");
                entity.Accounts = _db.Accounts.ToList();
                Logger.Instance.DataFlow($"Pull list of Accounts from DB");
                entity.Metrics = RefreshAccountMetrics(entity);
                Logger.Instance.DataFlow($"Refresh Account metrics");
                entity.RebalanceReport = _calc.GetRebalancingAccountsReport(entity);
                Logger.Instance.DataFlow($"Get rebalancing accounts report");
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
            }


            return entity;
        }

        public Account Details(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Details");
                Logger.Instance.DataFlow($"Pull Account with Id of {entity.Account.Id} from DB");
                return _db.Accounts.FirstOrDefault(a => a.Id == entity.Account.Id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(AccountDTO entity)
        {
            try
            {
                entity.Account.BalanceLimit = entity.Account.BalanceLimit;
                entity.Account.BalanceSurplus = entity.Account.BalanceSurplus;
                entity.Account.RequiredSavings = entity.Account.RequiredSavings;

                _db.Accounts.Add(entity.Account);
                Logger.Instance.DataFlow($"New Account added to data context");

                _db.SaveChanges();
                Logger.Instance.DataFlow($"Save changes to DB");


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Edit");
                _db.Entry(entity.Account).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Save Account changes to data context");
                _db.SaveChanges();
                Logger.Instance.DataFlow($"Save changes to DB");
                _calc.Update();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private AccountMetrics RefreshAccountMetrics(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Refresh Account metrics");
                AccountMetrics metrics = new AccountMetrics();

                metrics.LargestBalance = entity.Accounts.Max(a => a.Balance);
                metrics.SmallestBalance = entity.Accounts.Min(a => a.Balance);
                if (entity.Accounts.Count > 0)
                    metrics.AverageBalance = entity.Accounts.Sum(a => a.Balance) / entity.Accounts.Count;
                else
                    metrics.AverageBalance = 0;
                var incomeTransactions = _db.Transactions.Where(t => t.Type == TransactionTypesEnum.Income);
                var oldestIncomeTransaction = incomeTransactions.OrderBy(t => t.Date).FirstOrDefault();
                var daysAgo = 0;
                if (oldestIncomeTransaction != null)
                    daysAgo = (DateTime.Today - oldestIncomeTransaction.Date).Days;
                var monthsAgo = daysAgo / 30 < 1 ? 1 : daysAgo / 30;
                

                metrics.MonthlySurplus = (incomeTransactions.Sum(t => t.Amount) / monthsAgo) - (entity.Accounts.Sum(a => a.PaycheckContribution) * 2);
                metrics.LargestSurplus = entity.Accounts.Max(a => a.BalanceSurplus);
                metrics.SmallestSurplus = entity.Accounts.Min(a => a.BalanceSurplus);
                var surplusAccounts = entity.Accounts.Where(a => a.BalanceSurplus > 0).ToList().Count; if (surplusAccounts > 0)
                    metrics.AverageSurplus = entity.Accounts.Sum(a => a.BalanceSurplus) / surplusAccounts;
                metrics.TotalBalance = entity.Accounts.Sum(a => a.Balance);

                Logger.Instance.DataFlow($"Return Account metrics");
                return metrics;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
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
