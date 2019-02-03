using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Metrics;

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
    }
}
