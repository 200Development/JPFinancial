using System;

namespace JPFData.Models
{
    public class Expense
    {
        public Expense()
        {
            Due = DateTime.Today;
            Amount = decimal.Zero;
            BillId = 0;
            CreditAccountId = 0;
            IsPaid = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Due { get; set; }
        public decimal Amount { get; set; }
        public int BillId { get; set; }
        public int CreditAccountId { get; set; }
        public bool IsPaid { get; set; }
    }
}