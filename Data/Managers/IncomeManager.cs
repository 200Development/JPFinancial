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
    /// Manages all Income communication between the application and the database
    /// </summary>
    public class IncomeManager
    {
        /*
        MANAGER STRUCTURE
        private fields
        constructors
        public fields
        public methods
        private methods
        */
        private readonly ApplicationDbContext _db;


        public IncomeManager()
        {
            _db = new ApplicationDbContext();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public IncomeDTO Get(IncomeDTO entity)
        {
            try
            {
                entity.Paychecks = _db.Paychecks.ToList();
                entity.Metrics = RefreshIncomeMetrics(entity);
                return entity;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        //TODO: look into making dynamic instead of specific to paychecks
        public Paycheck GetPaycheck(IncomeDTO entity)
        {
            try
            {
                return _db.Paychecks.Find(entity.Paycheck.Id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public IncomeDTO Details(IncomeDTO entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(IncomeDTO entity)
        {
            try
            {
                _db.Paychecks.Add(entity.Paycheck);
                _db.SaveChanges();
                var paycheck = entity.Paycheck;
                if (entity.AutoTransferPaycheckContributions)
                    if (!AutoTransferPaycheckContributions(paycheck)) return false;

                AddIncomeToPoolAccount(paycheck);

                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Edit(IncomeDTO entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Update(Paycheck entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Delete(Paycheck entity)
        {
            try
            {
                var paycheck = _db.Paychecks.Find(entity.Id);
                var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount != null) poolAccount.Balance -= paycheck.NetPay;
                _db.Entry(poolAccount).State = EntityState.Modified;
                _db.Paychecks.Remove(paycheck);
                _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }


        private bool AutoTransferPaycheckContributions(Paycheck paycheck)
        {
            try
            {
                if (paycheck.NetPay <= 0) return true; //only return false when exception is thrown
                var accountsWithContributions = _db.Accounts.Where(a => a.PaycheckContribution != null && a.PaycheckContribution > 0).ToList();
                var totalContributions = accountsWithContributions.Sum(a => a.PaycheckContribution);
                if (totalContributions > paycheck.NetPay)
                {
                }

                foreach (var account in accountsWithContributions)
                {
                    if (!AddContributionTransaction(paycheck, account)) continue;
                }


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool AddContributionTransaction(Paycheck paycheck, Account account)
        {
            try
            {
                if (account.PaycheckContribution == null) return false;
                var contribution = (decimal)account.PaycheckContribution;
                paycheck.NetPay -= contribution;
                account.Balance += contribution;
                _db.Entry(paycheck).State = EntityState.Unchanged;
                _db.Entry(account).State = EntityState.Modified;


                var newTransaction = new Transaction();
                newTransaction.Date = paycheck.Date;
                newTransaction.Payee = $"Transfer to {account.Name}";
                newTransaction.Category = CategoriesEnum.PaycheckContribution;
                newTransaction.Memo = "Paycheck Contribution";
                newTransaction.Type = TransactionTypesEnum.Transfer;
                newTransaction.DebitAccountId = account.Id;
                newTransaction.CreditAccountId = null;
                newTransaction.Amount = contribution;
                newTransaction.PaycheckId = null;
                _db.Transactions.Add(newTransaction);

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private bool AddIncomeToPoolAccount(Paycheck paycheck)
        {
            try
            {
                if (paycheck.NetPay <= 0) return true; //only return false when exception is thrown
                var poolAccount = _db.Accounts.FirstOrDefault(a => a.IsPoolAccount);

                if (poolAccount == null) return true;
                poolAccount.Balance += paycheck.NetPay;
                _db.Entry(poolAccount).State = EntityState.Modified;


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private static IncomeMetrics RefreshIncomeMetrics(IncomeDTO entity)
        {
            try
            {
                IncomeMetrics metrics = new IncomeMetrics();

                var monthlyIncome = entity.Paychecks.GroupBy(p => p.Date.Month).Select(m => new { m.Key, Sum = m.Sum(i => i.NetPay) }).ToList();

                if (monthlyIncome.Count == 0) return null;
                metrics.AverageMonthlyIncome = monthlyIncome.Sum(m => m.Sum) / monthlyIncome.Count;
                metrics.AverageWeeklyIncome = metrics.AverageMonthlyIncome / 4;
                metrics.ProjectedAnnualIncome = metrics.AverageMonthlyIncome *= 12;

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
