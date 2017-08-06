using JPFinancial.Models.Interfaces;
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
        public DateTime DueDate { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal AmountDue { get; set; }

        [Required, Display(Name = "Frequency")]
        public Frequency PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Required]
        public virtual bool IsLate { get; set; }

        [Required]
        public virtual bool IsMandatory { get; set; }
    }
}