﻿using System;
using System.Collections.Generic;
using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    //TODO: Need Clearer Terminology
    public class Loan : ILoan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Origination Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LoanOriginationDate { get; set; }

        //TODO: Create method to handle payment due dates based on payment frequency
        [Display(Name = "Next Due Date"), DataType(DataType.Date)]
        public DateTime NextDueDate { get; set; }

        public int Term { get; set; }

        [Display(Name = "Term Classification")]
        public TermClassification TermClassification { get; set; }

        [Display(Name = "Original Balance"), DataType(DataType.Currency)]
        public decimal OriginalLoanAmount { get; set; }

        [Display(Name = "Principal Balance"), DataType(DataType.Currency)]
        public decimal PrincipalBalance { get; set; }

        [Display(Name = "Outstanding Balance"), DataType(DataType.Currency)]
        public decimal OutstandingBalance { get; set; }

        public decimal APR { get; set; }

        [Display(Name = "Accrued Interest"), DataType(DataType.Currency)]
        public decimal AccruedInterest { get; set; }

        [Display(Name = "Capitalized Interest"), DataType(DataType.Currency)]
        public decimal CapitalizedInterest { get; set; }

        [Display(Name = "Compound Frequency")]
        public FrequencyEnum CompoundFrequency { get; set; }

        [Display(Name = "Payment"), DataType(DataType.Currency)]
        public decimal Payment { get; set; }

        [Display(Name = "# of Payments")]
        public int Payments { get; set; }

        [Display(Name = "Payment Frequency")]
        public FrequencyEnum PaymentFrequency { get; set; }

        [Display(Name = "Due Day (Day of Month)")]
        public DaysOfMonth DueDayOfMonth { get; set; }
    }
}