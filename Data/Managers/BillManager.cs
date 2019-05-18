using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFData.Managers
{
    public class BillManager
    {
        private readonly ApplicationDbContext _db;
        private readonly string _userId;


        public BillManager()
        {
            _db = new ApplicationDbContext();
            _userId = Global.Instance.User.Id;
        }


        public BillViewModel GetAllBills(BillViewModel billVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Get");
                billVM.Bills = _db.Bills.Where(b => b.UserId == _userId).ToList();
                Logger.Instance.DataFlow($"Pull list of Bills from DB");
                billVM.Metrics = RefreshBillMetrics(billVM);
                Logger.Instance.DataFlow($"Refresh Bill metrics");

                
                return billVM;
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
                Logger.Instance.DataFlow($"Pull Account with ID {id} from DB and set to AccountViewModel.Entity.Account");
                return _db.Bills.Find(id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return null;
            }
        }

        public bool Create(BillViewModel billVM)
        {
            try
            {
                if (!AddBillToExpenses(billVM.Bill)) return false;

                _db.Bills.Add(billVM.Bill);
                Logger.Instance.DataFlow($"New Bill added to data context");

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

        public bool Edit(BillViewModel billVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Edit");
                _db.Entry(billVM.Bill).State = EntityState.Modified;
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
                Logger.Instance.Info($"Flagged to remove bill with id of {bill.Id} from DB");


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
                return _db.Accounts.Where(a => a.UserId == _userId).ToList();
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }

        //TODO: Refactor or redesign to prevent having to use new Expense Class
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

        private BillMetrics RefreshBillMetrics(BillViewModel billVM)
        {
            BillMetrics metrics = new BillMetrics();

            try
            {
                metrics.LargestBalance = billVM.Bills.Max(b => b.AmountDue);
                metrics.SmallestBalance = billVM.Bills.Min(b => b.AmountDue);
                metrics.TotalBalance = billVM.Bills.Sum(b => b.AmountDue);
                metrics.AverageBalance = billVM.Bills.Sum(b => b.AmountDue) / billVM.Bills.Count;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new BillMetrics();
            }


            return metrics;
        }

        public Bill Details(BillViewModel billVM)
        {
            try
            {
                Logger.Instance.DataFlow($"Details");
                Logger.Instance.DataFlow($"Pull Bill with Id of {billVM.Bill.Id} from DB");
                return _db.Bills.FirstOrDefault(b => b.Id == billVM.Bill.Id);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }
        }
    }
}
