﻿using System;
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
                Logger.Instance.DataFlow($"Get");
                entity.Accounts = _db.Accounts.ToList();
                Logger.Instance.DataFlow($"Pull list of Accounts from DB");
                entity.Metrics = RefreshAccountMetrics(entity);
                Logger.Instance.DataFlow($"Refresh Account metrics");
                entity.RebalanceReport = Calculations.GetRebalancingAccountsReport(entity);
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

        public bool Edit(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Edit");
                _db.Entry(entity.Account).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Save Account changes to data context");
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
        
        private AccountMetrics RefreshAccountMetrics(AccountDTO entity)
        {
            try
            {
                Logger.Instance.DataFlow($"Refresh Account metrics");
                AccountMetrics metrics = new AccountMetrics();

                metrics.LargestBalance = entity.Accounts.Max(a => a.Balance);
                metrics.SmallestBalance = entity.Accounts.Min(a => a.Balance);
                metrics.AverageBalance = entity.Accounts.Sum(a => a.Balance) / entity.Accounts.Count;

                metrics.LargestSurplus = entity.Accounts.Max(a => a.BalanceSurplus ?? 0m);
                metrics.SmallestSurplus = entity.Accounts.Min(a => a.BalanceSurplus ?? 0m);
                metrics.AverageSurplus = entity.Accounts.Sum(a => a.BalanceSurplus ?? 0m) / entity.Accounts
                                             .Where(a => a.BalanceSurplus != null && a.BalanceSurplus != 0m).ToList().Count;
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
    }
}
