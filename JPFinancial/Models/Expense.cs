using JPFinancial.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Expense : ISalaryLineItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int SalaryId { get; set; }
        [Display(Name = "Pre-Taxed?")]
        public bool IsPreTax { get; set; }
    }
}