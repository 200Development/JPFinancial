﻿using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Deduction
    {
        public Deduction()
        {
            Amount = 0.0m;;
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