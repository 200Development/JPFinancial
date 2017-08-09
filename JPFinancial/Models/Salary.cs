﻿using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        [Required, Display(Name = "Payee")]
        public Company Company { get; set; }

        [Required, Display(Name = "Pay Type")]
        public PayType PayType { get; set; }

        [Required, Display(Name = "Pay Frequency")]
        public Frequency PayFrequency { get; set; }

        [Required, Display(Name = "Gross Pay"), DataType(DataType.Currency)]
        public decimal? GrossPay { get; set; }
    }
}