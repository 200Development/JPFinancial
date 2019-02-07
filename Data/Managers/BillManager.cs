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
                Console.WriteLine(e);
                throw;
            }


            return entity;
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

        public IEnumerable<OutstandingBill> GetOutstandingBills()
        {
            try
            {
                var ret = new List<OutstandingBill>();
                Logger.Instance.DataFlow($"Get");
                var bills = _db.Bills.ToList();
                Logger.Instance.DataFlow($"Pulled list of Bills from DB");
                var outstandingBills = bills.Where(b => b.IsPaid == false).ToList();
                Logger.Instance.DataFlow($"Sorted Bills to only return ones that are outstanding");

                foreach (var bill in outstandingBills)
                {
                    var newBill = new OutstandingBill();
                    newBill.Id = bill.Id;
                    newBill.Name = $"{bill.Name} - {bill.AmountDue} Due {bill.DueDate.ToShortDateString()}";

                    ret.Add(newBill);
                }

                return ret;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(e);
                return new List<OutstandingBill>();
            }
        }
    }
}
