using System;

namespace JPFinancial.Models.Interfaces
{
    public interface ILiability
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime DueDate { get; set; }
        decimal AmountDue { get; set; }
        Frequency PaymentFrequency { get; set; }
        int AccountId { get; set; }
        Account Account { get; set; }
    }
}