using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFinancial.Models.Enumerations;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.ViewModels
{
    public class LoanViewModel : ILoan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Origination Date"), DataType(DataType.Date)]
        public DateTime LoanOriginationDate { get; set; }

        public DateTime NextDueDate { get; set; }

        public int Term { get; set; }

        public TermClassification TermClassification { get; set; }

        [Display(Name = "Original Balance"), DataType(DataType.Currency)]
        public decimal OriginalLoanAmount { get; set; }

        [Display(Name = "Principal Balance"), DataType(DataType.Currency)]
        public decimal PrincipalBalance { get; set; }

        [Display(Name = "Outstanding Balance"), DataType(DataType.Currency)]
        public decimal OutstandingBalance { get; set; }

        public decimal APR { get; set; }

        public decimal AccruedInterest { get; set; }

        public decimal CapitalizedInterest { get; set; }

        public FrequencyEnum CompoundFrequency { get; set; }

        [Display(Name = "Payment"), DataType(DataType.Currency)]
        public decimal Payment { get; set; }

        [Display(Name = "Fees"), DataType(DataType.Currency)]
        public decimal? Fees { get; set; }

        public int Payments { get; set; }

        public FrequencyEnum PaymentFrequency { get; set; }

        [Display(Name = "Daily Interest"), DataType(DataType.Currency)]
        public decimal DailyInterestCost { get; set; }

        [Display(Name = "Due Day (Day of Month)")]
        public DaysOfMonthEnum DueDayOfMonthEnum { get; set; }

        [Display(Name = "Expense Ratio")]
        public decimal? ExpenseRatio { get; set; }

        [Display(Name = "Savings Ratio")]
        public decimal? SavingsRatio { get; set; }

        [Display(Name = "Emergency Fund Coverage")]
        public decimal? EmergencyFundCoverage { get; set; }

        [Display(Name = "28/36 Rule")]
        public decimal? Rule2836 { get; set; }

        [Display(Name = "Savings Needed For Retirement")]
        public decimal? SavingsNeededForRetirement { get; set; }
    }
}