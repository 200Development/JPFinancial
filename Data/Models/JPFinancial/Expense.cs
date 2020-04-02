using System;
using JPFData.Interfaces;

namespace JPFData.Models.JPFinancial
{
    public class Expense : IExpense
    {
        public Expense()
        {
            Due = DateTime.Today;
            Amount = 0.0m;;
            BillId = 0;
            CreditAccountId = 0;
            IsPaid = false;
            if (Global.Instance.User != null)
                UserId = Global.Instance.User.Id;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Due { get; set; }
        public decimal Amount { get; set; }
        public int BillId { get; set; }
        public int CreditAccountId { get; set; }
        public bool IsPaid { get; set; }
        public string UserId { get; set; }
    }
}