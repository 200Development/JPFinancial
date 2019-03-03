using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.Managers
{
    public class BillManager
    {
        private readonly ApplicationDbContext _db;


        public BillManager()
        {
            _db = new ApplicationDbContext();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public BillDTO Get(BillDTO entity)
        {
            try
            {
                entity.Bills = _db.Bills.ToList();
                entity.Metrics = RefreshBillMetrics(entity);
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                throw;
            }


            return entity;
        }

        public bool Create(BillDTO entity)
        {
            try
            {
                if (!AddBillToExpenses(entity.Bill)) return false;

                _db.Bills.Add(entity.Bill);
                Logger.Instance.DataFlow($"New Bill added to data context");

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
                return false;
            }
        }

        public BillMetrics RefreshBillMetrics(BillDTO entity)
        {
            BillMetrics metrics = new BillMetrics();

            try
            {
                metrics.LargestBalance = entity.Bills.Max(b => b.AmountDue);
                metrics.SmallestBalance = entity.Bills.Min(b => b.AmountDue);
                metrics.TotalBalance = entity.Bills.Sum(b => b.AmountDue);
                metrics.AverageBalance = entity.Bills.Sum(b => b.AmountDue) / entity.Bills.Count;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return metrics;
        }

        //public IEnumerable<OutstandingExpense> GetOutstandingBills()
        //{
        //    try
        //    {
        //        var ret = new List<OutstandingExpense>();
        //        Logger.Instance.DataFlow($"Get");
        //        var bills = _db.Bills.ToList();
        //        Logger.Instance.DataFlow($"Pulled list of Bills from DB");
        //        var outstandingBills = bills.Where(b => b.IsPaid == false).ToList();
        //        Logger.Instance.DataFlow($"Sorted Bills to only return ones that are outstanding");

        //        foreach (var bill in outstandingBills)
        //        {
        //            var newBill = new OutstandingExpense();
        //            newBill.Id = bill.Id;
        //            newBill.Name = $"{bill.Name} - {bill.AmountDue} Due {bill.DueDate.ToShortDateString()}";

        //            ret.Add(newBill);
        //        }

        //        return ret;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Instance.Error(e);
        //        return new List<OutstandingExpense>();
        //    }
        //}
        public Bill Details(BillDTO entity)
        {
            throw new NotImplementedException();
        }

        public bool Edit(BillDTO entity)
        {
            throw new NotImplementedException();
        }
    }
}
