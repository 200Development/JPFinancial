using JPFinancial.Models.Enumerations;
using JPFinancial.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Bill : ILiability
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Date)]
        [JsonProperty(PropertyName = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required, DataType(DataType.Currency)]
        [JsonProperty(PropertyName = "Amount Due")]
        public decimal AmountDue { get; set; }

        [Required, Display(Name = "Frequency")]
        public FrequencyEnum PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Required]
        [JsonProperty(PropertyName = "Late?")]
        public virtual bool IsLate { get; set; }

        [Required]
        [JsonProperty(PropertyName = "Mandatory Expense?")]
        public virtual bool IsMandatory { get; set; }
    }
}