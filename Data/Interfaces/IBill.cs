using System;
using JPFData.Enumerations;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface IBill
    {
        int Id { get; set; }
        string UserId { get; set; }
        string Name { get; set; }
        DateTime DueDate { get; set; }
        decimal AmountDue { get; set; }
        FrequencyEnum PaymentFrequency { get; set; }
        int AccountId { get; set; }
        Account Account { get; set; }
    }
}