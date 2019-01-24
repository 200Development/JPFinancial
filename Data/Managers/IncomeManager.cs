using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Metrics;

namespace JPFData.Managers
{
    public class IncomeManager
    {
        private readonly ApplicationDbContext _db;


        public IncomeManager()
        {
            _db = new ApplicationDbContext();
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }


        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public IncomeDTO Get()
        {
            return Get(new IncomeDTO());
        }

        public IncomeDTO Get(IncomeDTO entity)
        {
            try
            {
                entity.Paychecks = _db.Paychecks.ToList();
                entity.Metrics = RefreshIncomeMetrics(entity);
            }
            catch (Exception)
            {
               //ignore
            }

            return entity;
        }

        private static IncomeMetrics RefreshIncomeMetrics(IncomeDTO entity)
        {
            IncomeMetrics metrics = new IncomeMetrics();

            var monthlyIncome = entity.Paychecks.GroupBy(p => p.Date.Month).Select(m => new { m.Key, Sum = m.Sum(i => i.NetPay) });
            metrics.AverageMonthlyIncome = monthlyIncome.Sum(m => m.Sum) / monthlyIncome.Count();
            metrics.AverageWeeklyIncome = metrics.AverageMonthlyIncome / 4;
            metrics.ProjectedAnnualIncome = metrics.AverageMonthlyIncome *= 12;

            return metrics;
        }
    }
}
