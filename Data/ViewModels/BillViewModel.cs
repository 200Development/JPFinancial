using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.ViewModels
{
    public class BillViewModel
    {
        public BillViewModel()
        {
            Bill = new Bill();
            Bills = new List<Bill>();
            Metrics = new BillMetrics();
        }

        public Bill Bill { get; set; }
        public List<Bill> Bills { get; set; }
        public List<Account> Accounts { get; set; }
        public BillMetrics Metrics { get; set; }
    }
}