using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
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
                entity.Accounts = _db.Accounts.ToList();
                entity.Metrics = RefreshAccountMetrics(entity);
                entity.RebalanceReport = Calculations.GetRebalancingAccountsReport(entity);
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
                return _db.Accounts.FirstOrDefault(a => a.Id == entity.Account.Id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
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
                Logger.Instance.Error(e);
                return false;
            }
        }
        
        /// <summary>
        /// Updates Account balances in the database.  Uses surplus balances to pay off deficits
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public AccountDTO Update(AccountDTO entity)
        {
            try
            {
                // set paycheck contributions to suggested contributions
                if (!_calc.UpdatePaycheckContributions()) return entity;

                entity.Accounts = _db.Accounts.ToList();

                if (!_calc.UpdateRequiredBalanceForBills()) return entity;

                if (!_calc.UpdateBalanceSurplus()) return entity;


                _db.SaveChanges();
                return entity;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public AccountDTO Rebalance(AccountDTO entity)
        {
            try
            {
                if (!_calc.PoolSurplus()) return entity;
                if (!_calc.RebalanceAccounts()) return entity;


                _db.SaveChanges();
                Update(entity);  //is this necessary or overkill?
                return entity;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }


        private AccountMetrics RefreshAccountMetrics(AccountDTO entity)
        {
            try
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
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }
    }
}
