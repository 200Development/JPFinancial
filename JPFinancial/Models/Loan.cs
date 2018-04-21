using System;
using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    //TODO: Need Clearer Terminology
    public class Loan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Origination Date")]
        public DateTime LoanOriginationDate { get; set; }

        //TODO: Create method to handle payment due dates based on payment frequency
        [Display(Name = "Next Due Date")]
        public DateTime NextDueDate { get; set; }

        public int Term { get; set; }

        [Display(Name = "Term Classification")]
        public TermClassification TermClassification { get; set; }

        [Display(Name = "Original Balance")]
        public decimal OriginalLoanAmount { get; set; }

        [Display(Name = "Principal Balance")]
        public decimal PrincipalBalance { get; set; }

        [Display(Name = "Outstanding Balance")]
        public decimal OutstandingBalance { get; set; }

        public decimal APR { get; set; }

        [Display(Name = "Accrued Interest")]
        public decimal AccruedInterest { get; set; }

        [Display(Name = "Capitalized Interest")]
        public decimal CapitalizedInterest { get; set; }

        [Display(Name = "Compound Frequency")]
        public FrequencyEnum CompoundFrequency { get; set; }

        [Display(Name = "Payment")]
        public decimal Payment { get; set; }

        [Display(Name = "Fees")]
        public decimal? Fees { get; set; }

        [Display(Name = "# of Payments")]
        public int Payments { get; set; }

        [Display(Name = "Payment Frequency")]
        public FrequencyEnum PaymentFrequency { get; set; }
    }
}