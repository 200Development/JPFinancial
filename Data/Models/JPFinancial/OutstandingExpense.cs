
using System;

namespace JPFData.Models.JPFinancial
{
    public class OutstandingExpense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AmountDue { get; set; }
    }
}
