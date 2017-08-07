using JPFinancial.Models;
using System.Collections.Generic;

namespace JPFinancial.ViewModels
{
    public class CreateBillViewModel
    {
        public string Name { get; set; }

        public bool IsMandatory { get; set; }

        public decimal AmountDue { get; set; }

        public string DueDate { get; set; }

        public Frequency PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public IList<Account> Accounts { get; set; }
    }
}