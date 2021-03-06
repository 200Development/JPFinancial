﻿using System;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Transaction 
    {
        public Transaction()
        {
            Date = DateTime.Today;
            Payee = string.Empty;
            Memo = string.Empty;
            CreditAccountId = 0;
            DebitAccountId = 0;
            SelectedCreditCardAccountId = 0;
            SelectedExpenseId = 0;
            Amount = decimal.Zero;
            UsedCreditCard = false;
        }

        [Key]
        public int Id { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public int? CreditAccountId { get; set; }
        public int? DebitAccountId { get; set; }
        public int? SelectedCreditCardAccountId { get; set; }
        public int? SelectedExpenseId { get; set; }
        public DateTime? SelectedBillDueDate { get; set; }
        //public Paycheck Paycheck { get; set; }
        //public int? PaycheckId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "From")]
        public Account CreditAccount { get; set; }

        [Display(Name = "To")]
        public Account DebitAccount { get; set; }
   
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Charged to Credit Card?")]
        public bool UsedCreditCard { get; set; }
    }
}