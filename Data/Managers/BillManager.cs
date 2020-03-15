using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFData.Managers
{
    /// <summary>
    /// Manages all read/write to database Bills Table
    /// </summary>
    public class BillManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;
        private readonly Calculations _calc;

        public BillManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance.User != null ? Global.Instance.User.Id : string.Empty;
            _calc = new Calculations();
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

        //TODO: Refactor or redesign to prevent having to instantiate new Expense Class
        private bool AddBillToExpenses(Bill bill)
        {
            try
            {
                var expense = new Expense();
                expense.Name = bill.Name;
                expense.BillId = bill.Id;
                expense.Amount = bill.AmountDue;
                expense.Due = bill.DueDate;
                expense.IsPaid = false;
                expense.UserId = _userId;

                _db.Expenses.Add(expense);
                Logger.Instance.DataFlow($"New Expense added to data context");

                _db.SaveChanges();
                Logger.Instance.DataFlow($"Saved changes to DB");


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
                var paycheckContributionsDict = _calc.GetPaycheckContributionsDict();
                var accountManager = new AccountManager();
                var account = new Account();

                // Update paycheck contribution for all accounts
                foreach (var paycheckContribution in paycheckContributionsDict)
                {
                    try
                    {
                        account = updatedAccounts.Find(a => string.Equals(a.Name, paycheckContribution.Key, StringComparison.CurrentCultureIgnoreCase));
                        var accountIndex = -1;

                        if (account == null)
                        {
                            account = new Account {Name = paycheckContribution.Key};
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


                        // iterate through all updated accounts and set state to modified to save to database
                        var accounts = accountManager.GetAllAccounts();

                        foreach (var updatedAccount in updatedAccounts)
                        {
                            try
                            {

                                account = accounts.Find(a => string.Equals(a.Name, updatedAccount.Name, StringComparison.CurrentCultureIgnoreCase));

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

        public IEnumerable<OutstandingExpense> GetOutstandingBills()
        {
            try
            {
                var ret = new List<OutstandingExpense>();
                Logger.Instance.DataFlow($"Get");

                // Get all bill expenses that have not yet been paid
                var expenses = _db.Expenses.Where(e => e.UserId == Global.Instance.User.Id).Where(e => e.BillId > 0 && !e.IsPaid).ToList();
                Logger.Instance.DataFlow($"Pulled list of bill expenses from DB that haven't been paid");
                //var outstandingBills = bills.Where(b => b.IsPaid == false).ToList();

                foreach (var expense in expenses)
                {
                    var newExpense = new OutstandingExpense();
                    newExpense.Id = expense.Id;
                    newExpense.Name = $"{expense.Name} - {expense.Amount} Due {expense.Due.ToShortDateString()}";
                    newExpense.DueDate = expense.Due;
                    newExpense.AmountDue = expense.Amount;

                    ret.Add(newExpense);
                }

                return ret;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new List<OutstandingExpense>();
            }
        }

        public decimal GetOutstandingExpenseTotal()
        {
            try
            {
                var outstandingBills = GetOutstandingBills();
                return outstandingBills.Sum(b => b.AmountDue);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return 0m;
            }
        }
    }
}
