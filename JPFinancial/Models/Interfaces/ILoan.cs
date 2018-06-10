using System;
using JPFinancial.Models.Enumerations;

namespace JPFinancial.Models.Interfaces
{
    public interface ILoan
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime LoanOriginationDate { get; set; }
        DateTime NextDueDate { get; set; }
        int Term { get; set; }
        TermClassification TermClassification { get; set; }
        decimal OriginalLoanAmount { get; set; }
        decimal PrincipalBalance { get; set; }
        decimal OutstandingBalance { get; set; }
        decimal APR { get; set; }
        decimal AccruedInterest { get; set; }
        decimal CapitalizedInterest { get; set; }
        FrequencyEnum CompoundFrequency { get; set; }
        decimal Payment { get; set; }
        int Payments { get; set; }
        FrequencyEnum PaymentFrequency { get; set; }
        DaysOfMonthEnum DueDayOfMonthEnum { get; set; }
    }
}