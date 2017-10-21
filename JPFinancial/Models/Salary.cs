﻿using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Payee")]
        public string Payee { get; set; }

        [Required, Display(Name = "Pay Type")]
        public PayType PayType { get; set; }

        [Required, Display(Name = "Pay Frequency")]
        public Frequency PayFrequency { get; set; }

        [Display(Name = "Gross Pay"), DataType(DataType.Currency)]
        public decimal? GrossPay { get; set; }

        [Display(Name = "Net Income"), DataType(DataType.Currency)]
        public decimal? NetIncome { get; set; }
    }
}