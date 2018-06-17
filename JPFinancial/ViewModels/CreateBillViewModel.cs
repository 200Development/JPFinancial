using System;
using JPFinancial.Models;
using JPFinancial.Models.Enumerations;
using System.Collections.Generic;

namespace JPFinancial.ViewModels
{
    public class CreateBillViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMandatory { get; set; }

        public decimal AmountDue { get; set; }

        public DateTime DueDate { get; set; }

        public FrequencyEnum PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public IList<Account> Accounts { get; set; }

        public Account Account { get; set; }
    }
}