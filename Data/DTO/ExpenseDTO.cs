using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.DTO
{
    public class ExpenseDTO
    {
        public List<Expense> Expenses { get; set; }
        public ExpenseMetrics Metrics { get; set; }
    }
}
