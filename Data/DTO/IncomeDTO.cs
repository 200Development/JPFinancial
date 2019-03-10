using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.DTO
{
    public class IncomeDTO
    {
        public IncomeDTO()
        {
            Paycheck = new Paycheck();
            Paychecks = new List<Paycheck>();
            Metrics = new IncomeMetrics();
        }

        public Paycheck Paycheck { get; set; }
        public List<Paycheck> Paychecks { get; set; }
        public IncomeMetrics Metrics { get; set; }
        public bool AutoTransferPaycheckContributions { get; set; }
    }
}
