using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class BillDTO
    {
        public BillDTO()
        {
            Bill = new Bill();
            Bills = new List<Bill>();
            Metrics = new BillMetrics();
        }

        public Bill Bill { get; set; }
        public List<Bill> Bills { get; set; }
        public BillMetrics Metrics { get; set; }
    }
}
