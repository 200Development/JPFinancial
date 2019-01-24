using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class CreateBillViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMandatory { get; set; }

        public decimal AmountDue { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        public FrequencyEnum PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public IList<Account> Accounts { get; set; }

        public Account Account { get; set; }
    }
}