﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;

namespace JPFData
{
    public class Calculations
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        /// <summary>
        /// Returns the last day of the month for the provided date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime LastDayOfMonth(DateTime date)
        {
            try
            {
                var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
                Logger.Instance.Calculation($"LastDayOfMonth({date:d}) returned {lastDay:d}");
                return lastDay;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new DateTime();
            }
        }

        /// <summary>
        /// Returns the first day of the month for the provided year and month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DateTime FirstDayOfMonth(int year, int month)
        {
            //TODO: change to take datetime as parameter
            try
            {
                var firstDay = new DateTime(year, month, 1);
                Logger.Instance.Calculation($"FirstDayOfMonth({year}, {month}) returned {firstDay}");
                return firstDay;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new DateTime();
            }
        }

        public decimal FutureValue(DateTime futureDate, decimal? netPay)
        {
            try
            {
                Logger.Instance.Calculation($"FutureValue");
                var payperiods = PayPeriodsTilDue(futureDate);
                var date = DateTime.Today;
                var billManager = new BillManager();
                var billsFromDb = billManager.GetAllBills();

                var bills = new Dictionary<string, string>
                {
                    {"currentDate", DateTime.Today.ToShortDateString()},
                    {"endDate", DateTime.Today.Day <= 14
                        ? new DateTime(date.Year, date.Month, 15).ToShortDateString()
                        : new DateTime(date.Year, date.Month, LastDayOfMonth(date).Day).ToShortDateString()
                    },
                    {"periodCosts", "0"},
                    {"totalCosts", "0"},
                    {"totalSavings", "0"}
                };

                foreach (var bill in billsFromDb)
                {
                    bills.Add(bill.Name, bill.DueDate.ToShortDateString());
                }

                for (var i = 0; i < payperiods; i++)
                {

                    bills = UpdateBillDueDates(bills);
                    bills = UpdateTotalCosts(bills);
                    SetCurrentAndEndDate(bills);
                    decimal? savings = Convert.ToDecimal(bills["totalSavings"]);
                    var periodCosts = Convert.ToDecimal(bills["periodCosts"]);

                    savings += netPay - periodCosts;
                    bills["totalSavings"] = savings.ToString();
                }
                //var cost = Convert.ToDecimal(bills["periodCosts"]);
                var save = Convert.ToDecimal(bills["totalSavings"]);
                Logger.Instance.Calculation($"FutureValue({futureDate:d}, {netPay}) returned {save}");

                return save;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal GetMonthlyIncome()
        {
            try
            {
                Logger.Instance.Calculation($"GetMonthlyIncome");
                var incomePerPayPeriod = Convert.ToDecimal(_db.Salaries.Sum(s => s.NetIncome));
                var paymentFrequency = _db.Salaries.Select(s => s.PayFrequency).FirstOrDefault();
                var monthlyIncome = 0.00m;

                switch (paymentFrequency)
                {
                    case FrequencyEnum.Weekly:
                        monthlyIncome = incomePerPayPeriod * 4;
                        Logger.Instance.Calculation($"Monthly income for weekly pay is {monthlyIncome} (income per pay {incomePerPayPeriod} * 4)");
                        break;
                    case FrequencyEnum.SemiMonthly:
                        monthlyIncome = incomePerPayPeriod * 2;
                        Logger.Instance.Calculation($"Monthly income for semi-monthly pay is {monthlyIncome} (income per pay {incomePerPayPeriod} * 2)");
                        break;
                    case FrequencyEnum.Monthly:
                        monthlyIncome = incomePerPayPeriod;
                        Logger.Instance.Calculation($"Monthly income for monthly pay is {monthlyIncome} (income per pay {incomePerPayPeriod})");
                        break;
                    default:
                        monthlyIncome = incomePerPayPeriod * 2;
                        Logger.Instance.Calculation($"Monthly income for semi-monthly pay is {monthlyIncome} (income per pay {incomePerPayPeriod} * 2)");
                        break;
                }

                return monthlyIncome;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m;
            }
        }

        //TODO: Improve date range handling
        /// <summary>
        /// Returns summation of bills within the date range of the begin and end parameters  
        /// </summary>
        /// <param name="begin">start date of the date range</param>
        /// <param name="end">end date of the date range</param>
        /// <param name="onlyMandatory">sumates only mandatory expenses</param>
        /// <returns></returns>
        public decimal ExpensesByDateRange(DateTime begin, DateTime end, bool onlyMandatory = false)
        {
            try
            {
                Logger.Instance.Calculation($"Expenses by DateRange");
                var billManager = new BillManager();
                var bills = billManager.GetAllBills();
                var expenses = 0m;

                foreach (var bill in bills)
                {
                    //if (bill.DueDate.Date < beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;
                    Logger.Instance.Calculation($"{bill.Name} original due date is {dueDate:d}");
                    //TODO: Fix semi-monthly bills being added 3 times (31st, 16th, 1st)
                    while (newDueDate >= begin)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(-1);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(-7);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(-14);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(-1);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(-15);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(-3);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(-6);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(-1);
                                Logger.Instance.Calculation($"{bill.Name} new due date is {newDueDate:d}");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        // adds expense only if the bill due date falls within the date range
                        if (newDueDate < begin || newDueDate >= end) continue;
                        expenses += bill.AmountDue;
                        Logger.Instance.Calculation($"Expenses: {expenses} added to {bill.Name}.AmountDue");
                    }
                }
                return expenses;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m;
            }
        }

        public decimal DiscretionarySpendingByDateRange(DateTime begin, DateTime end)
        {
            try
            {
                Logger.Instance.Calculation($"DiscretionarySpendingByDateRange");
                var transactionManager = new TransactionManager();
                var transactions = transactionManager.GetTransactionsBetweenDates(begin, end);
                var billManager = new BillManager();
                var bills = billManager.GetAllBills();
                var isBill = false;
                var ret = 0m;

                foreach (var transaction in transactions)
                {
                    foreach (var bill in bills)
                    {
                        // If the bill name matches the transaction payee, count the transaction as a bill (mandatory expense)
                        if (bill.Name.Equals(transaction.Payee))
                        {
                            isBill = true;
                            Logger.Instance.Calculation($"{transaction.Amount}: {transaction.Payee} transaction is a bill");
                        }
                        else
                            Logger.Instance.Calculation($"{transaction.Amount}: {transaction.Payee} transaction is not a bill");
                    }

                    // If transaction is not a bill, add to discretionary spending total
                    if (isBill) continue;

                    ret += transaction.Amount;
                    Logger.Instance.Calculation($"{transaction.Amount} added to discretionary spending total");
                }
                Logger.Instance.Calculation($"{ret} total discretionary spending from {begin:d} to {end:d}");
                return ret;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m;
            }
        }

        public static decimal DailyInterest(Loan loan)
        {
            try
            {
                Logger.Instance.Calculation($"DailyInterest");
                var dailyInterestRate = (loan.APR / 100) / (decimal)364.25;
                var dailyInterest = dailyInterestRate * loan.OutstandingBalance;
                Logger.Instance.Calculation($"{loan.Name} loan daily interest = {dailyInterest} (dailyInterestRate {dailyInterestRate} * outstandingBalance {loan.OutstandingBalance})");
                return dailyInterest;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m;
            }
        }

        public static decimal MonthlyInterest(Loan loan)
        {
            try
            {
                Logger.Instance.Calculation($"MonthlyInterest");
                var monthlyInterestRate = (loan.APR / 100) / 12;
                var monthlyInterest = monthlyInterestRate * loan.OutstandingBalance;
                Logger.Instance.Calculation($"{loan.Name} loan monthly interest = {monthlyInterest} (monthlyInterestRate {monthlyInterestRate} * outstandingBalance {loan.OutstandingBalance})");
                return monthlyInterest;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0.0m;
            }
        }

        public AccountRebalanceReport GetRebalancingAccountsReport()
        {
            try
            {
                Logger.Instance.Calculation($"GetRebalancingAccountsReport");
                AccountRebalanceReport report = new AccountRebalanceReport();
                var accountManager = new AccountManager();
                var accounts = accountManager.GetAllAccounts();

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
        /// Updates Account balances in the database.  Uses surplus balances to pay off deficits
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update()
        {
            try
            {
                var requiredSavingsDict = GetRequiredSavingsDict();
                var paycheckContributionsDict = GetPaycheckContributionsDict();
                var updatedAccounts = new List<Account>();

                // Update required savings for all accounts and add updated accounts to updatedAccounts to track changes
                foreach (var requiredSavings in requiredSavingsDict)
                {
                    try
                    {
                        var account = updatedAccounts.Find(a => string.Equals(a.Name, requiredSavings.Key, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (account == null)
                        {
                            account = new Account();
                            account.Name = requiredSavings.Key;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, requiredSavings.Key, StringComparison.CurrentCultureIgnoreCase));

                        if (!(requiredSavings.Value >= account.RequiredSavings)) continue;


                        if (accountIndex >= 0)
                            updatedAccounts[accountIndex].RequiredSavings = requiredSavings.Value;
                        else
                        {
                            account.RequiredSavings = requiredSavings.Value;
                            updatedAccounts.Add(account);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // Update paycheck contribution for all accounts
                foreach (var paycheckContribution in paycheckContributionsDict)
                {
                    try
                    {
                        var account = updatedAccounts.Find(a => string.Equals(a.Name, paycheckContribution.Key, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (account == null)
                        {
                            account = new Account();
                            account.Name = paycheckContribution.Key;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, paycheckContribution.Key, StringComparison.CurrentCultureIgnoreCase));

                        if (!(paycheckContribution.Value >= account.PaycheckContribution)) continue;


                        if (accountIndex >= 0)
                            updatedAccounts[accountIndex].PaycheckContribution = paycheckContribution.Value;
                        else
                        {
                            account.PaycheckContribution = paycheckContribution.Value;
                            updatedAccounts.Add(account);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // iterate through all updated accounts and set state to modified to save to database
                foreach (var updatedAccount in updatedAccounts)
                {
                    try
                    {
                        var accountManager = new AccountManager();
                        var accounts = accountManager.GetAllAccounts();
                        var account = accounts.Find(a => string.Equals(a.Name, updatedAccount.Name, StringComparison.CurrentCultureIgnoreCase));

                        // shouldn't ever be null since updatedAccounts comes from Accounts in DB
                        account.PaycheckContribution = updatedAccount.PaycheckContribution;
                        account.RequiredSavings = updatedAccount.RequiredSavings;

                        var requiredSurplus = account.Balance - account.RequiredSavings;

                        if (requiredSurplus <= 0)
                            account.BalanceSurplus = requiredSurplus;
                        else
                        {
                            if (account.Balance - account.BalanceLimit <= 0)
                                account.BalanceSurplus = 0;
                            else
                                account.BalanceSurplus = account.Balance - account.BalanceLimit;
                        }


                        _db.Entry(account).State = EntityState.Modified;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // save changes to the database
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

        public bool Rebalance()
        {
            try
            {
                var accountManager = new AccountManager();
                var updatedAccounts = new List<Account>();
                var accounts = accountManager.GetAllAccounts();
                var poolAccount = accountManager.GetPoolAccount();
                var requiredSavingsDict = GetRequiredSavingsDict();

                // update required savings for all accounts
                foreach (var requiredSavings in requiredSavingsDict)
                {
                    try
                    {
                        var account = updatedAccounts.Find(a => string.Equals(a.Name, requiredSavings.Key, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (account == null)
                        {
                            account = new Account();
                            account.Name = requiredSavings.Key;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, requiredSavings.Key, StringComparison.CurrentCultureIgnoreCase));

                        if (!(requiredSavings.Value >= account.RequiredSavings)) continue;


                        if (accountIndex >= 0)
                            updatedAccounts[accountIndex].RequiredSavings = requiredSavings.Value;
                        else
                        {
                            account.RequiredSavings = requiredSavings.Value;
                            updatedAccounts.Add(account);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                if (poolAccount == null) throw new Exception("Pool account has not been assigned");

                // transfer balance surplus's from each account to the pool account
                foreach (Account account in accounts)
                {
                    try
                    {
                        if (account.Id == poolAccount.Id || account.ExcludeFromSurplus || account.BalanceSurplus <= 0) continue;
                        var updatedAccount = updatedAccounts.Find(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (updatedAccount == null)
                        {
                            updatedAccount = new Account();
                            updatedAccount.Name = account.Name;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));


                        if (updatedAccount.BalanceSurplus == 0) continue;
                        if (accountIndex >= 0)
                            updatedAccounts[accountIndex].Balance -= account.BalanceSurplus;
                        else
                            updatedAccount.Balance -= updatedAccount.BalanceSurplus;
                        poolAccount.Balance += updatedAccount.BalanceSurplus;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // cover any account deficits from the surplus in the pool account 
                foreach (var account in accounts)
                {
                    try
                    {
                        if (poolAccount.Balance <= 0) continue;
                        if (account.ExcludeFromSurplus || account.BalanceSurplus >= 0) continue;
                        var updatedAccount = updatedAccounts.Find(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (updatedAccount == null)
                        {
                            updatedAccount = new Account();
                            updatedAccount.Name = account.Name;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));


                        var deficit = account.BalanceSurplus * -1;

                        // If pool account doesn't have enough to cover the full deficit, use what is left
                        if (poolAccount.Balance < deficit)
                        {
                            var balance = poolAccount.Balance;
                            if (accountIndex >= 0)
                                updatedAccounts[accountIndex].Balance += balance;
                            else
                                updatedAccount.Balance += balance;
                            poolAccount.Balance -= balance;
                        }
                        else // Make account whole
                        {
                            if (accountIndex >= 0)
                                updatedAccounts[accountIndex].Balance += deficit;
                            else
                                updatedAccount.Balance += deficit;
                            poolAccount.Balance -= deficit;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // update required savings for each account
                foreach (var account in accounts)
                {
                    try
                    {
                        var surplus = account.Balance - account.RequiredSavings;
                        var updatedAccount = updatedAccounts.Find(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (updatedAccount == null)
                        {
                            updatedAccount = new Account();
                            updatedAccount.Name = account.Name;
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => string.Equals(a.Name, account.Name, StringComparison.CurrentCultureIgnoreCase));


                        // adds any funds
                        if (surplus <= 0)
                        {
                            // update the balance surplus of the account in updatedAccounts List if it exists or update the instantiated updatedAccount
                            if (accountIndex >= 0)
                                updatedAccounts[accountIndex].BalanceSurplus = surplus;
                            else
                                updatedAccount.BalanceSurplus = surplus;
                        }
                        else
                        {
                            if (account.Balance - account.BalanceLimit <= 0)
                            {
                                if (accountIndex >= 0)
                                    updatedAccounts[accountIndex].BalanceSurplus = 0;
                                else
                                    updatedAccount.BalanceSurplus = 0;
                            }
                            else
                            {
                                if (accountIndex >= 0)
                                    updatedAccounts[accountIndex].BalanceSurplus = account.Balance - account.BalanceLimit;
                                else
                                    updatedAccount.BalanceSurplus = account.Balance - account.BalanceLimit;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // iterate through all updated accounts and set state to modified to save to database
                foreach (var updatedAccount in updatedAccounts)
                {
                    try
                    {
                        var account = accounts.Find(a => string.Equals(a.Name, updatedAccount.Name, StringComparison.CurrentCultureIgnoreCase));

                        // shouldn't ever be null since updatedAccounts comes from Accounts in DB
                        account.PaycheckContribution = updatedAccount.PaycheckContribution;
                        account.RequiredSavings = updatedAccount.RequiredSavings;

                        var requiredSurplus = account.Balance - account.RequiredSavings;

                        if (requiredSurplus <= 0)
                            account.BalanceSurplus = requiredSurplus;
                        else
                        {
                            if (account.Balance - account.BalanceLimit <= 0)
                                account.BalanceSurplus = 0;
                            else
                                account.BalanceSurplus = account.Balance - account.BalanceLimit;
                        }


                        _db.Entry(account).State = EntityState.Modified;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // save changes to the database 
                try
                {
                    _db.Entry(poolAccount).State = EntityState.Modified;
                    if (_db.ChangeTracker.HasChanges())
                        _db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Update Account.RequiredBalance for Accounts used in Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool UpdateRequiredSavings(Transaction transaction)
        {
            try
            {
                Logger.Instance.Calculation($"UpdateRequiredSavings");
                var savingsAccountBalances = new Dictionary<string, decimal>();
                var expenseManager = new ExpenseManager();
                var unpaidExpenses = expenseManager.GetAllUnpaidExpenses();

                foreach (var expense in unpaidExpenses)
                {
                    var bill = _db.Bills.FirstOrDefault(b => b.Id == expense.BillId);
                    if (bill == null)
                    {
                        Logger.Instance.Debug($"No Bill found WHERE expense.BillId = {expense.BillId}");
                        Logger.Instance.DataFlow($"No Bill found WHERE expense.BillId = {expense.BillId}");
                        continue;
                    }

                    bill.Account = _db.Accounts.FirstOrDefault(a => a.Id == bill.AccountId);
                    if (bill.Account == null) continue;
                    var billTotal = expense.Amount; // Use info from Expense and not Bill to account for when the current Bill.Amount differs from past amounts
                    var dueDate = expense.Due;
                    var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                    var savePerPaycheck = 0.0m;
                    var save = 0.0m;

                    // Calculate how much to save each pay period
                    if (dueDate > DateTime.Today)
                    {
                        switch (bill.PaymentFrequency)
                        {
                            case FrequencyEnum.Annually:
                                savePerPaycheck = billTotal / 24;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.SemiAnnually:
                                savePerPaycheck = billTotal / 12;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.Quarterly:
                                savePerPaycheck = billTotal / 6;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.SemiMonthly:
                                savePerPaycheck = billTotal / 4;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.Monthly:
                                savePerPaycheck = billTotal / 2;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.BiWeekly:
                                savePerPaycheck = billTotal;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            case FrequencyEnum.Weekly:
                                savePerPaycheck = billTotal * 2;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                            default:
                                savePerPaycheck = billTotal / 2;
                                Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                break;
                        }

                        save = Math.Round(billTotal - payPeriodsLeft * savePerPaycheck, 2);
                    }
                    else
                        save = expense.Amount;

                    // required savings = bill amount due - (how many pay periods before due date * how much to save per pay period)

                    Logger.Instance.Calculation($"{bill.Account.Name} - [{Math.Round(billTotal, 2)}] [{bill.DueDate:d}] [{payPeriodsLeft}(ppl)] [{Math.Round(savePerPaycheck, 2)}(spp)] [{Math.Round(save, 2)}(req save)]");

                    if (savingsAccountBalances.ContainsKey(bill.Account.Name))
                        savingsAccountBalances[bill.Account.Name] += save;
                    else
                        savingsAccountBalances.Add(bill.Account.Name, save);
                    ////savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                }

                // update each account that has a bill credited to it 
                if (transaction.CreditAccount != null)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != transaction.CreditAccount.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }

                    if (valuesFound)
                    {
                        // If required savings doesn't need updating, save a call to the DB
                        if (transaction.CreditAccount.RequiredSavings != totalSavings)
                        {
                            transaction.CreditAccount.RequiredSavings = totalSavings;
                            Logger.Instance.Calculation(
                                $"{transaction.CreditAccount.Name}.RequiredSavings = {Math.Round(totalSavings, 2)}");
                            _db.Entry(transaction.CreditAccount).State = EntityState.Modified;
                        }
                    }
                }
                if (transaction.DebitAccount != null)
                {
                    var valuesFound = false;
                    decimal totalSavings = 0;

                    foreach (var savings in savingsAccountBalances)
                    {
                        if (savings.Key != transaction.DebitAccount.Name) continue;
                        totalSavings += savings.Value;
                        valuesFound = true;
                    }

                    if (valuesFound)
                    {
                        // If required savings doesn't need updating, save a call to the DB
                        if (transaction.DebitAccount.RequiredSavings != totalSavings)
                        {
                            transaction.DebitAccount.RequiredSavings = totalSavings;
                            Logger.Instance.Calculation(
                                $"{transaction.DebitAccount.Name}.RequiredSavings = {Math.Round(totalSavings, 2)}");
                            _db.Entry(transaction.DebitAccount).State = EntityState.Modified;
                        }
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

        public decimal UpdateBalanceSurplus(Account account)
        {
            try
            {
                if (account.Balance < 0.0m)
                    return account.BalanceSurplus = account.Balance;

                if (account.BalanceLimit > 0)
                    return account.BalanceSurplus = account.Balance > account.BalanceLimit
                        ? account.Balance - account.BalanceLimit
                        : decimal.Zero;

                return account.Balance - account.RequiredSavings;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return decimal.Zero;
            }
        }

        /// <summary>
        /// Updates database Bills.DueDate if the previous due date has passed
        /// </summary>
        public void UpdateBillDueDates()
        {
            try
            {
                Logger.Instance.Calculation($"UpdateBillDueDates");
                var billManager = new BillManager();
                var bills = billManager.GetAllBills();
                var beginDate = DateTime.Today;

                foreach (var bill in bills)
                {
                    if (bill.DueDate.Date > beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;

                    /* Updates bill due date to the current due date
                       while loop handles due date updates, regardless of how out of date they are */
                    while (newDueDate < beginDate)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    bill.DueDate = newDueDate;
                    _db.Entry(bill).State = EntityState.Modified;
                    Logger.Instance.Calculation($"{bill.Name} due date of {dueDate:d} updated to {newDueDate:d}");

                    if (!AddNewExpenseToDb(bill)) return;
                }


                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
            }
        }

        private Dictionary<string, decimal> GetRequiredSavingsDict()
        {
            try
            {
                var savingsAccountBalances = new Dictionary<string, decimal>();
                ExpenseManager expenseManager = new ExpenseManager();
                AccountManager accountManager = new AccountManager();
                BillManager billManager = new BillManager();
                List<Expense> unpaidExpenses = expenseManager.GetAllUnpaidExpenses();

                //foreach (var bill in _db.Bills.ToList())
                foreach (var expense in unpaidExpenses)
                {
                    try
                    {
                        var bill = billManager.GetBill(expense.BillId);
                        if (bill == null)
                        {
                            Logger.Instance.Debug($"No Bill found WHERE expense.BillId = {expense.BillId}");
                            Logger.Instance.DataFlow($"No Bill found WHERE expense.BillId = {expense.BillId}");
                            continue;
                        }

                        bill.Account = accountManager.GetAccount(bill.AccountId);
                        if (bill.Account == null) continue;
                        var billTotal = expense.Amount; // Use info from Expense and not Bill to account for when the current Bill.Amount differs from past amounts
                        var dueDate = expense.Due;
                        var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                        var savePerPaycheck = 0.0m;
                        var save = 0.0m;

                        // Calculate how much to save each pay period
                        if (dueDate > DateTime.Today)
                        {
                            switch (bill.PaymentFrequency)
                            {
                                case FrequencyEnum.Annually:
                                    savePerPaycheck = billTotal / 24;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.SemiAnnually:
                                    savePerPaycheck = billTotal / 12;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Quarterly:
                                    savePerPaycheck = billTotal / 6;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.SemiMonthly:
                                    savePerPaycheck = billTotal / 4;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Monthly:
                                    savePerPaycheck = billTotal / 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.BiWeekly:
                                    savePerPaycheck = billTotal;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Weekly:
                                    savePerPaycheck = billTotal * 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                default:
                                    savePerPaycheck = billTotal / 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                            }

                            save = Math.Round(billTotal - payPeriodsLeft * savePerPaycheck, 2);
                        }
                        else
                            save = expense.Amount;

                        // required savings = bill amount due - (how many pay periods before due date * how much to save per pay period)

                        Logger.Instance.Calculation($"{bill.Account.Name} - [{Math.Round(billTotal, 2)}] [{bill.DueDate:d}] [{payPeriodsLeft}(ppl)] [{Math.Round(savePerPaycheck, 2)}(spp)] [{Math.Round(save, 2)}(req save)]");

                        if (savingsAccountBalances.ContainsKey(bill.Account.Name))
                            savingsAccountBalances[bill.Account.Name] += save;
                        else
                            savingsAccountBalances.Add(bill.Account.Name, save);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                        throw;
                    }
                }


                return savingsAccountBalances;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private Dictionary<string, decimal> GetPaycheckContributionsDict()
        {
            try
            {
                var accountManager = new AccountManager();
                var accounts = accountManager.GetAllAccounts();

                var billManager = new BillManager();
                var bills = billManager.GetAllBills();

                var accountContribution = new Dictionary<string, decimal>();

                //Zeros out all accounts req paycheck contributions
                foreach (var account in accounts)
                {
                    account.PaycheckContribution = decimal.Zero;
                }

                // update suggested paycheck contributions for bills
                foreach (var bill in bills)
                {
                    var billTotal = bill.AmountDue;
                    Logger.Instance.Calculation($"{billTotal} due on {bill.DueDate} for {bill.Name}");

                    // get the account assigned to the bill
                    bill.Account = accounts.FirstOrDefault(a => a.Id == bill.AccountId);
                    if (bill.Account != null && bill.Account.ExcludeFromSurplus) continue;

                    //TODO: Needs to account for all pay frequencies
                    //TODO: Suggested contribution assumes payday twice a month.  need to update to include other options
                    if (bill.Account == null) continue;
                    var contribution = 0.0m;
                    switch (bill.PaymentFrequency)
                    {
                        case FrequencyEnum.Annually:
                            contribution = billTotal / 24;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.SemiAnnually:
                            contribution = billTotal / 12;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.Quarterly:
                            contribution = billTotal / 6;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.SemiMonthly: // every 2 months
                            contribution = billTotal / 4;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.Monthly:
                            contribution = billTotal / 2;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.Weekly:
                            contribution = billTotal * 2;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.BiWeekly:
                            contribution = billTotal;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                        case FrequencyEnum.Daily:
                            break;
                        default:
                            contribution = billTotal / 2;
                            if (accountContribution.ContainsKey(bill.Account.Name))
                                accountContribution[bill.Account.Name] += contribution;
                            else
                                accountContribution.Add(bill.Account.Name, contribution);
                            Logger.Instance.Calculation($"{Math.Round(contribution, 2)} added to {bill.Account.Name}.SuggestedContribution");
                            break;
                    }
                }

                return accountContribution;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// Database update if dbSave = true, else EntityState.Modified.
        /// Update the required savings for each Account.
        /// Required savings = 
        /// </summary>
        private bool UpdateRequiredSavings()
        {
            // public because runs on startup with dbSave = true
            try
            {
                var savingsAccountBalances = new Dictionary<string, decimal>();
                ExpenseManager expenseManager = new ExpenseManager();
                AccountManager accountManager = new AccountManager();
                BillManager billManager = new BillManager();
                List<Expense> unpaidExpenses = expenseManager.GetAllUnpaidExpenses();
                List<Account> accounts = accountManager.GetAllAccounts();

                //foreach (var bill in _db.Bills.ToList())
                foreach (var expense in unpaidExpenses)
                {
                    try
                    {
                        var bill = billManager.GetBill(expense.BillId);
                        if (bill == null)
                        {
                            Logger.Instance.Debug($"No Bill found WHERE expense.BillId = {expense.BillId}");
                            Logger.Instance.DataFlow($"No Bill found WHERE expense.BillId = {expense.BillId}");
                            continue;
                        }

                        bill.Account = accountManager.GetAccount(bill.AccountId);
                        if (bill.Account == null) continue;
                        var billTotal = expense.Amount; // Use info from Expense and not Bill to account for when the current Bill.Amount differs from past amounts
                        var dueDate = expense.Due;
                        var payPeriodsLeft = PayPeriodsTilDue(dueDate);
                        var savePerPaycheck = 0.0m;
                        var save = 0.0m;

                        // Calculate how much to save each pay period
                        if (dueDate > DateTime.Today)
                        {
                            switch (bill.PaymentFrequency)
                            {
                                case FrequencyEnum.Annually:
                                    savePerPaycheck = billTotal / 24;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.SemiAnnually:
                                    savePerPaycheck = billTotal / 12;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Quarterly:
                                    savePerPaycheck = billTotal / 6;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.SemiMonthly:
                                    savePerPaycheck = billTotal / 4;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Monthly:
                                    savePerPaycheck = billTotal / 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.BiWeekly:
                                    savePerPaycheck = billTotal;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                case FrequencyEnum.Weekly:
                                    savePerPaycheck = billTotal * 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                                default:
                                    savePerPaycheck = billTotal / 2;
                                    Logger.Instance.Calculation($"{expense.Name} save/paycheck = {Math.Round(savePerPaycheck, 2)} to {bill.Account.Name} account");
                                    break;
                            }

                            save = Math.Round(billTotal - payPeriodsLeft * savePerPaycheck, 2);
                        }
                        else
                            save = expense.Amount;

                        // required savings = bill amount due - (how many pay periods before due date * how much to save per pay period)

                        Logger.Instance.Calculation($"{bill.Account.Name} - [{Math.Round(billTotal, 2)}] [{bill.DueDate:d}] [{payPeriodsLeft}(ppl)] [{Math.Round(savePerPaycheck, 2)}(spp)] [{Math.Round(save, 2)}(req save)]");

                        if (savingsAccountBalances.ContainsKey(bill.Account.Name))
                            savingsAccountBalances[bill.Account.Name] += save;
                        else
                            savingsAccountBalances.Add(bill.Account.Name, save);
                        ////savingsAccountBalances.Add(new KeyValuePair<string, decimal>(bill.Account.Name, save));
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                // update each account that has a bill credited to it 
                foreach (var account in accounts)
                {
                    try
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

                        // If required savings doesn't need updating, save a call to the DB
                        if (account.RequiredSavings == totalSavings) continue;

                        account.RequiredSavings = totalSavings;
                        Logger.Instance.Calculation($"{account.Name}.RequiredSavings = {Math.Round(totalSavings, 2)}");
                        _db.Entry(account).State = EntityState.Modified;
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }


                //_db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Database update if dbSave = true, else EntityState.Modified.
        /// Update the balance surplus for each Account.
        /// Balance surplus = balance - required savings.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool UpdateAllAccountSurpluses()
        {
            // public because runs on startup with dbSave = true
            try
            {
                Logger.Instance.Calculation($"UpdateBalanceSurplus");
                var accountManager = new AccountManager();
                var accounts = accountManager.GetAllAccounts();

                foreach (var account in accounts)
                {
                    var requiredSurplus = account.Balance - account.RequiredSavings;

                    //TODO: Add comment for clarity
                    if (requiredSurplus <= 0)
                        account.BalanceSurplus = requiredSurplus;
                    else
                    {
                        if (account.Balance - account.BalanceLimit <= 0)
                            account.BalanceSurplus = 0;
                        else
                            account.BalanceSurplus = account.Balance - account.BalanceLimit;
                    }


                    _db.Entry(account).State = EntityState.Modified;
                }


                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Entity update.
        /// Move all Account surpluses to the designated pool Account.
        /// Pool Account balance += Accounts' surplus
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>If Account balances updated and transactions added to db successfully</returns>
        private bool PoolSurplus()
        {
            try
            {
                Logger.Instance.Calculation($"PoolSurplus");
                AccountManager accountManager = new AccountManager();
                var accountsUpdated = false;
                var accounts = accountManager.GetAllAccounts();
                var poolAccount = accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) throw new Exception("Pool account has not been assigned");

                foreach (Account account in accounts)
                {
                    if (account.Id == poolAccount.Id || account.ExcludeFromSurplus || account.BalanceSurplus <= 0) continue;
                    account.Balance -= account.BalanceSurplus;
                    poolAccount.Balance += account.BalanceSurplus;

                    _db.Entry(account).State = EntityState.Modified;
                    _db.Entry(poolAccount).State = EntityState.Modified;
                    accountsUpdated = true;
                }

                if (accountsUpdated)
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
        /// Entity update.
        /// Update balance for Accounts with deficits.
        /// While (pool.balance > 0) take deficit amount from pool and add to deficit Account balance.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool RebalanceAccounts()
        {
            try
            {
                Logger.Instance.Calculation($"RebalanceAccounts");
                AccountManager accountManager = new AccountManager();
                var accountsUpdated = false;
                var accounts = accountManager.GetAllAccounts();
                var poolAccount = accounts.FirstOrDefault(a => a.IsPoolAccount);
                if (poolAccount == null) throw new Exception("Pool account has not been assigned");

                foreach (var account in accounts)
                {
                    if (poolAccount.Balance <= 0) break;
                    if (account.ExcludeFromSurplus || account.BalanceSurplus >= 0) continue;
                    var deficit = account.BalanceSurplus * -1;

                    // If pool account doesn't have enough to cover the full deficit, use what is left
                    if (poolAccount.Balance < deficit)
                    {
                        var balance = poolAccount.Balance;
                        account.Balance += balance;
                        poolAccount.Balance -= balance;


                        // DB update deficit Account balance
                        _db.Entry(account).State = EntityState.Modified;
                        _db.Entry(poolAccount).State = EntityState.Modified;
                        accountsUpdated = true;
                    }
                    else // Make account whole
                    {
                        account.Balance += deficit;
                        poolAccount.Balance -= deficit;

                        _db.Entry(account).State = EntityState.Modified;
                        _db.Entry(poolAccount).State = EntityState.Modified;
                        accountsUpdated = true;
                    }
                }


                if (accountsUpdated)
                    _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        private Dictionary<string, string> UpdateTotalCosts(Dictionary<string, string> billsDictionary)
        {
            try
            {
                Logger.Instance.Calculation($"UpdateTotalCosts");
                var billManager = new BillManager();
                var bills = billManager.GetAllBills();
                var currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                var endDate = Convert.ToDateTime(billsDictionary["endDate"]);
                var expenses = 0.0m;
                billsDictionary["periodCosts"] = "0";

                foreach (var bill in billsDictionary)
                {
                    if (bill.Key == "currentDate" || bill.Key == "endDate" || bill.Key == "periodCosts" ||
                        bill.Key == "totalSavings" || bill.Key == "totalCosts") continue;

                    var dueDate = Convert.ToDateTime(bill.Value);
                    if (!(dueDate >= currentDate && dueDate <= endDate)) continue;

                    expenses += bills.Where(b => b.Name == bill.Key).Select(b => b.AmountDue).FirstOrDefault();
                    Logger.Instance.Calculation($"{expenses} added to {bill.Key}");
                }

                var billCosts = Convert.ToDecimal(billsDictionary["totalCosts"]);
                billsDictionary["totalCosts"] = (expenses + billCosts).ToString(CultureInfo.InvariantCulture);
                Logger.Instance.Calculation($"{expenses + billCosts} added to total costs (expenses: {expenses} + bill costs: {billCosts})");
                billsDictionary["periodCosts"] = expenses.ToString(CultureInfo.InvariantCulture);
                Logger.Instance.Calculation($"expenses: {expenses} added to period costs");

                return billsDictionary;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        private bool AddNewExpenseToDb(Bill bill)
        {
            try
            {
                if (ExpenseExists(bill))
                {
                    Logger.Instance.DataFlow($"{bill.Name} due {bill.DueDate} already exists in Expense DB table");
                    return false;
                }

                var newExpense = new Expense();
                newExpense.Name = bill.Name;
                newExpense.BillId = bill.Id;
                newExpense.Amount = bill.AmountDue;
                newExpense.Due = bill.DueDate;
                newExpense.IsPaid = false;

                _db.Expenses.Add(newExpense);
                Logger.Instance.DataFlow($"New Expense added to data context");

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

        private bool ExpenseExists(Bill bill)
        {
            try
            {
                return _db.Expenses.Where(e => e.BillId == bill.Id)
                    .Where(e => e.Due == bill.DueDate)
                    .Any(e => e.Amount == bill.AmountDue);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw new Exception("Error thrown checking for existing Expense", e);
            }
        }

        /// <summary>
        /// Returns Dictionary with due dates for all bills for calculating all expenses within a timeframe.  Used for calculating future savings
        /// </summary>
        /// <param name="billsDictionary"></param>
        /// <returns></returns>
        private Dictionary<string, string> UpdateBillDueDates(Dictionary<string, string> billsDictionary)
        {
            try
            {
                Logger.Instance.Calculation($"UpdateBillDueDates");
                var billManager = new BillManager();
                var bills = billManager.GetAllBills();
                var beginDate = Convert.ToDateTime(billsDictionary["currentDate"]);

                foreach (var bill in bills)
                {
                    if (bill.DueDate.Date > beginDate) continue;

                    var frequency = bill.PaymentFrequency;
                    var dueDate = bill.DueDate;
                    var newDueDate = dueDate;
                    Logger.Instance.Calculation($"{bill.Name} due date {dueDate:d}");
                    while (newDueDate < beginDate)
                    {
                        switch (frequency)
                        {
                            case FrequencyEnum.Daily:
                                newDueDate = newDueDate.AddDays(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Weekly:
                                newDueDate = newDueDate.AddDays(7);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.BiWeekly:
                                newDueDate = newDueDate.AddDays(14);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Monthly:
                                newDueDate = newDueDate.AddMonths(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiMonthly:
                                newDueDate = newDueDate.AddDays(15);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Quarterly:
                                newDueDate = newDueDate.AddMonths(3);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.SemiAnnually:
                                newDueDate = newDueDate.AddMonths(6);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            case FrequencyEnum.Annually:
                                newDueDate = newDueDate.AddYears(1);
                                Logger.Instance.Calculation($"New due date {newDueDate:d}");
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        billsDictionary[bill.Name] = newDueDate.ToShortDateString();
                        Logger.Instance.Calculation($"Set due date {newDueDate:d}");
                    }
                }
                return billsDictionary;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        /// <summary>
        /// Returns how many times the user will get paid before a due date
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        private static int PayPeriodsTilDue(DateTime? dueDate)
        {
            try
            {
                Logger.Instance.Calculation($"PayPeriodsTilDue");
                var payPeriods = 0;
                var today = DateTime.Today;
                var month = DateTime.Today.Month;
                var year = DateTime.Today.Year;
                var firstDayOfMonth = new DateTime(year, month, 1);
                var firstPaycheckDate = new DateTime(year, month, 15);

                while (dueDate > today)
                {
                    if (today >= firstDayOfMonth && today <= firstPaycheckDate) // 1st - 15th of the month
                    {
                        payPeriods += 1;
                        today = firstPaycheckDate.AddDays(1);
                    }
                    else // 16th through the last day of the month
                    {
                        payPeriods += 1;
                        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
                        firstPaycheckDate = firstPaycheckDate.AddMonths(1);
                        today = firstDayOfMonth;
                    }
                }
                Logger.Instance.Calculation($"{payPeriods} pay periods until due date {dueDate:d}");
                return payPeriods - 1 < 0 ? 0 : payPeriods - 1;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        /// <summary>
        /// Returns Dictionary (string, string) with current and end (next pay period) dates set
        /// </summary>
        /// <param name="billsDictionary"></param>
        private void SetCurrentAndEndDate(IDictionary<string, string> billsDictionary)
        {
            try
            {
                Logger.Instance.Calculation($"SetCurrentAndEndDate");
                var currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                var endDate = Convert.ToDateTime(billsDictionary["endDate"]);
                Logger.Instance.Calculation($"CurrentDate: {currentDate}, EndDate: {endDate}");

                if (Convert.ToDateTime(billsDictionary["currentDate"]).Day <= 14)
                {
                    billsDictionary["currentDate"] = new DateTime(currentDate.Year, currentDate.Month, 16).ToShortDateString();
                    currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, LastDayOfMonth(currentDate).Day);
                    billsDictionary["endDate"] = endDate.ToShortDateString();
                    Logger.Instance.Calculation($"New CurrentDate: {currentDate}, EndDate: {endDate}");
                }
                else
                {
                    billsDictionary["currentDate"] =
                        FirstDayOfMonth(currentDate.AddMonths(1).Year, currentDate.AddMonths(1).Month)
                            .ToString(CultureInfo.InvariantCulture);
                    currentDate = Convert.ToDateTime(billsDictionary["currentDate"]);
                    endDate = new DateTime(currentDate.Year, currentDate.Month, 15);
                    billsDictionary["endDate"] = endDate.ToShortDateString();
                    Logger.Instance.Calculation($"New CurrentDate: {currentDate}, EndDate: {endDate}");
                    //TODO: simplify
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
            }
        }
    }
}