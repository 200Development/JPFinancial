using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class ExpenseDTO
    {
        public ExpenseDTO()
        {
            
        }


        public List<Expense> Expenses { get; set; }
        public ExpenseMetrics Metrics { get; set; }
    }
}
