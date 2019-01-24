using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Deduction
    {
        public Deduction()
        {
            Amount = decimal.Zero;
            BeforeTax = false;
        }

        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Deduction")]
        public DeductionsEnum Name { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency), Display(Name = "Year-To-Date")]
        public decimal? YTDAmount { get; set; }

        [Required, Display(Name = "Before-Tax")]
        public bool BeforeTax { get; set; }
    }
}