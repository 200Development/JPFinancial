using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Interfaces;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Accounts Table
    /// </summary>
    public class AccountManager : IAccountManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;


        public AccountManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;

            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

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
                account.BalanceSurplus = UpdateBalanceSurplus(account);

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
        
        public bool UpdateAccountsFromDashboard(IList<Account> accounts)
        {
            try
            {
                foreach (var account in accounts)
                {
                    if (account.Id <= 0)
                    {
                        var poolAccount = GetPoolAccount();
                        poolAccount.Balance = account.Balance;

                        _db.Entry(poolAccount).State = EntityState.Modified;
                    }
                    else
                    {
                        var dbAccount = GetAccount(account.Id);
                        dbAccount.Balance = account.Balance;

                        if (account.Balance > dbAccount.BalanceLimit)
                            dbAccount.BalanceLimit = account.Balance;

                        dbAccount.BalanceSurplus = UpdateBalanceSurplus(dbAccount);

                        _db.Entry(dbAccount).State = EntityState.Modified;
                    }
                }
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

        public Account GetEmergencyFundAccount()
        {
            try
            {
                return _db.Accounts.Where(a => a.UserId == _userId).FirstOrDefault(a => a.IsEmergencyFund);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public AccountMetrics GetMetrics()
        {
            try
            {
                //TODO: Needs Refactoring
                var metrics = new AccountMetrics();
                var accountManager = new AccountManager();
                var expenseManager = new ExpenseManager();
                var accounts = GetAllAccounts();
                var poolAccount = GetPoolAccount();
                var requiredSavingsDict = Calculations.GetRequiredSavingsDict();
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
                var outstandingExpenses = expenseManager.GetOutstandingExpensesTotal();

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

        public AccountRebalanceReport GetRebalancingAccountsReport()
        {
            try
            {
                Logger.Instance.Calculation($"GetRebalancingAccountsReport");
                AccountRebalanceReport report = new AccountRebalanceReport();
                var accounts = GetAllAccounts();

                //Clear out to prevent stacking
                report.AccountsWithSurplus.Clear();
                report.AccountsWithDeficit.Clear();
                report.Surplus = 0.0m;
                report.Deficit = 0.0m;
                foreach (var account in accounts)
                {
                    Logger.Instance.Calculation($"Name: {account.Name}; Accts. W/ Surplus: {report.AccountsWithSurplus.Count}; Accts. W/ Deficit: {report.AccountsWithDeficit.Count}; Surplus: {Math.Round(report.Surplus, 2)}; Deficit: {Math.Round(report.Deficit, 2)}; TotalSurplus: {Math.Round(report.TotalSurplus, 2)}; PaycheckSurplus: {Math.Round(report.PaycheckSurplus, 2)}");
                    // Get Accounts' Total Surplus/Deficit
                    var accountSurplus = account.Balance - account.RequiredSavings;
                    if (accountSurplus == 0) continue;
                    if (accountSurplus > 0)
                    {
                        report.AccountsWithSurplus.Add(account);
                        report.Surplus += accountSurplus;
                        Logger.Instance.Calculation($"{Math.Round(accountSurplus, 2)} surplus added to report.Surplus ({report.Surplus})");
                    }
                    else if (accountSurplus < 0)
                    {
                        report.AccountsWithDeficit.Add(account);
                        report.Deficit += accountSurplus;
                        Logger.Instance.Calculation($"{Math.Round(accountSurplus, 2)} deficit added to report.Deficit ({report.Deficit})");
                    }

                    if (!account.ExcludeFromSurplus)
                    {
                        report.TotalSurplus += accountSurplus;
                        Logger.Instance.Calculation($"{Math.Round(accountSurplus, 2)} added to report.TotalSurplus ({report.TotalSurplus})");
                    }

                    // Get Paycheck's Total Surplus/Deficit
                    if (account.ExcludeFromSurplus) continue;

                    var paycheckSurplus = account.PaycheckContribution - account.SuggestedPaycheckContribution;
                    if (paycheckSurplus == 0) continue;

                    report.PaycheckSurplus += paycheckSurplus;
                    Logger.Instance.Calculation($"{paycheckSurplus} paycheck surplus (contribution: {account.PaycheckContribution} - suggested: {account.SuggestedPaycheckContribution}) added to report.PaycheckSurplus ({report.Surplus})");
                }

                report.NewReport = true;
                return report;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new AccountRebalanceReport();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal UpdateBalanceSurplus(Account account)
        {
            try
            {
                // If account balance is < $0.00, balance surplus = balance.
                // If there's a balance limit AND the account balance is greater than the balance limit, balance surplus = any funds over the balance limit
                // If the balance limit is > balance, balance surplus = $0.00 since there is no surplus
                // If there's no balance limit, balance surplus = any funds over the required savings
                if (account.Balance < 0.0m)
                    return account.Balance;

                if (account.BalanceLimit > 0.0m)
                    return account.Balance > account.BalanceLimit
                        ? account.Balance - account.BalanceLimit
                        : 0.0m;

                return account.Balance - account.RequiredSavings;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m; ;
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
                poolAccount.Balance = 0.0m;
                poolAccount.IsPoolAccount = true;
                poolAccount.IsEmergencyFund = false;
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

        public void CheckAndCreateEmergencyFund()
        {
            try
            {
                var accounts = _db.Accounts.Where(a => a.UserId == _userId).ToList();
                if (accounts.Exists(a => a.IsEmergencyFund)) return;

                var efAccount = new Account();
                efAccount.Name = "Emergency Fund";
                efAccount.Balance = 0.0m;
                efAccount.IsPoolAccount = false;
                efAccount.IsEmergencyFund = true;
                efAccount.UserId = _userId;

                _db.Accounts.Add(efAccount);
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        /// <summary>
        /// Updates Account balances in the database.  Uses surplus balances to pay off deficits
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public bool Update()
        {
            try
            {
                var requiredSavingsDict = Calculations.GetRequiredSavingsDict();
                var paycheckContributionsDict = Calculations.GetPaycheckContributionsDict();


                foreach (var account in GetAllAccounts())
                {
                    try
                    {
                        var requiredSavings = requiredSavingsDict.FirstOrDefault(k => k.Key == account.Name).Value;
                        var paycheckContributions = paycheckContributionsDict.FirstOrDefault(p => p.Key == account.Name).Value;
                        account.RequiredSavings = requiredSavings > 0.00m ? requiredSavings : 0.00m;
                        account.PaycheckContribution = paycheckContributions > 0.00m ? paycheckContributions : 0.00m;
                        account.BalanceSurplus = UpdateBalanceSurplus(account);


                        _db.Entry(account).State = EntityState.Modified;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // save changes to the database
                if (_db.ChangeTracker.HasChanges())
                {
                    _db.SaveChanges();
                }



                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        public bool Rebalance()
        {
            try
            {
                var accountManager = new AccountManager();
                var poolAccount = accountManager.GetPoolAccount();
                var requiredSavingsDict = Calculations.GetRequiredSavingsDict();


                if (poolAccount == null) throw new Exception("Pool account has not been assigned");


                foreach (Account account in GetAllAccounts().Where(a => a.BalanceSurplus != 0.0m && !a.ExcludeFromSurplus))
                {
                    try
                    {
                        // if balance surplus is > $0.00, transfer to pool account.  If surplus is < 0, transfer funds from pool to cover account deficits
                        if (!account.ExcludeFromSurplus && account.BalanceSurplus > 0)
                        {
                            // transfer balance surplus's from each account to the pool account
                            account.Balance -= account.BalanceSurplus;
                            poolAccount.Balance += account.BalanceSurplus;

                            // set balance surplus to $0.00
                            account.BalanceSurplus = 0;
                        }
                        else if (!account.ExcludeFromSurplus && account.BalanceSurplus < 0)
                        {
                            // cover any account deficits from the surplus in the pool account 
                            var deficit = account.BalanceSurplus * -1;

                            // Use the rest of the pool money if there's not enough to cover the full deficit
                            if (poolAccount.Balance < deficit)
                            {
                                account.Balance += poolAccount.Balance;
                                poolAccount.Balance -= poolAccount.Balance;
                            }
                            else // Make account whole
                            {
                                account.Balance += deficit;
                                poolAccount.Balance -= deficit;
                            }
                        }


                        account.RequiredSavings = requiredSavingsDict.FirstOrDefault(k => k.Key == account.Name).Value;
                        account.BalanceSurplus = UpdateBalanceSurplus(account);


                        _db.Entry(account).State = EntityState.Modified;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }


                // save changes to the database
                _db.Entry(poolAccount).State = EntityState.Modified;
                if (_db.ChangeTracker.HasChanges())
                    _db.SaveChanges();


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }
    }
}
