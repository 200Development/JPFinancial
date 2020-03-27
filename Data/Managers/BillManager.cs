using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Bills Table
    /// </summary>
    public class BillManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;


        public BillManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;
        }


        public List<Bill> GetAllBills()
        {
            try
            {

                Logger.Instance.DataFlow($"Return list of Bills");
                return _db.Bills.Where(b => b.UserId == _userId).Include(b => b.Account).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public Bill GetBill(int? id)
        {
            try
            {
                Logger.Instance.DataFlow($"Pull Bill with ID {id} from DB and set to BillViewModel.Account");
                return _db.Bills.Find(id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(Bill bill)
        {
            try
            {
                if (!AddBill(bill)) return false;

                if (!AddBillToExpenses(bill)) return false;

                if (!UpdateAccountPaycheckContribution()) return false;

                Logger.Instance.DataFlow($"Saved changes to DB");

                
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }
        
        public bool Edit(Bill bill)
        {
            try
            {
                Logger.Instance.DataFlow($"Edit");
                _db.Entry(bill).State = EntityState.Modified;
                Logger.Instance.DataFlow($"Save Account changes to data context");
                _db.SaveChanges();
                Logger.Instance.DataFlow($"Save changes to DB");
                

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        public bool Delete(int billId)
        {
            try
            {
                List<Expense> expenses = _db.Expenses.Where(e => e.BillId == billId).ToList();
                foreach (Expense expense in expenses)
                {
                    _db.Expenses.Remove(expense);
                    Logger.Instance.Info($"Flagged to remove expense with id of {expense.Id} from DB");
                }

                Bill bill = _db.Bills.Find(billId);
                _db.Bills.Remove(bill);
                Logger.Instance.Info($"Bill with id of {bill.Id} has been flagged for removal from DB");


                _db.SaveChanges();


               return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        private bool AddBill(Bill bill)
        {
            try
            {
                _db.Bills.Add(bill);
                Logger.Instance.DataFlow($"New Bill added to data context");
                _db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }
        
        //TODO: Update to only update effected Accounts
        private bool UpdateAccountPaycheckContribution()
        {
            try
            {
                var updatedAccounts = new List<Account>();
                var paycheckContributionsDict = Calculations.GetPaycheckContributionsDict();
                var accountManager = new AccountManager();
                var account = new Account();

                // Update paycheck contribution for all accounts
                foreach (var paycheckContribution in paycheckContributionsDict)
                {
                    try
                    {
                        account = updatedAccounts.Find(a => String.Equals(a.Name, paycheckContribution.Key, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (account == null)
                        {
                            account = new Account {Name = paycheckContribution.Key};
                        }
                        else
                            accountIndex = updatedAccounts.FindIndex(a => String.Equals(a.Name, paycheckContribution.Key, StringComparison.CurrentCultureIgnoreCase));

                        if (!(paycheckContribution.Value >= account.PaycheckContribution)) continue;


                        if (accountIndex >= 0)
                            updatedAccounts[accountIndex].PaycheckContribution = paycheckContribution.Value;
                        else
                        {
                            account.PaycheckContribution = paycheckContribution.Value;
                            updatedAccounts.Add(account);
                        }


                        // iterate through all updated accounts and set state to modified to save to database
                        var accounts = accountManager.GetAllAccounts();

                        foreach (var updatedAccount in updatedAccounts)
                        {
                            try
                            {

                                account = accounts.Find(a => String.Equals(a.Name, updatedAccount.Name, StringComparison.CurrentCultureIgnoreCase));

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

                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error(e);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return false;
            }
        }

        //TODO:update to not take any parameters
        public BillMetrics GetBillMetrics()
        {
            List<Bill> bills = _db.Bills.Where(b => b.UserId == _userId).ToList();
            BillMetrics metrics = new BillMetrics();

            try
            {
                metrics.LargestBalance = bills.Max(b => b.AmountDue);
                metrics.SmallestBalance = bills.Min(b => b.AmountDue);
                metrics.TotalBalance = bills.Sum(b => b.AmountDue);
                metrics.AverageBalance = bills.Sum(b => b.AmountDue) / bills.Count;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new BillMetrics();
            }


            return metrics;
        }

        /// <summary>
        /// Updates database Bills.DueDate if the previous due date has passed
        /// </summary>
        public void UpdateBillDueDates()
        {
            try
            {
                Logger.Instance.Calculation($"UpdateBillDueDates");
                var bills = GetAllBills();
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

                    if (!AddBillToExpenses(bill)) return;
                }


                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
            }
        }

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

        /// <summary>
        /// Returns Dictionary with due dates for all bills for calculating all expenses within a timeframe.  Used for calculating future savings
        /// </summary>
        /// <param name="billsDictionary"></param>
        /// <returns></returns>
        public Dictionary<string, string> UpdateBillDueDates(Dictionary<string, string> billsDictionary)
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

        public Dictionary<string, string> UpdateTotalCosts(Dictionary<string, string> billsDictionary)
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

        private bool AddBillToExpenses(Bill bill)
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
                newExpense.CreditAccountId = bill.AccountId;
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
    }
}
