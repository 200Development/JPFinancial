using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class IncomeDTO
    {
        public IncomeDTO()
        {
            Paychecks = new List<Paycheck>();
            Metrics = new IncomeMetrics();
        }

        public List<Paycheck> Paychecks { get; set; }
        public IncomeMetrics Metrics { get; set; }
    }
}
