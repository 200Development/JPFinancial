using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.ViewModels
{
    public class CreateSalaryViewModel
    {
        [Required, Display(Name = "Payee")]
        public string Payee { get; set; }

        [Required, Display(Name = "Pay Type")]
        public PayType PayType { get; set; }

        [Required, Display(Name = "Pay Frequency")]
        public Frequency PayFrequency { get; set; }

        [Required, Display(Name = "Gross Pay"), DataType(DataType.Currency)]
        public decimal? GrossPay { get; set; }

        public string Expense { get; set; }

        [Display(Name = "Cost")]
        public decimal ExpenseAmount { get; set; }

        public string Benefit { get; set; }

        [Display(Name = "Amount")]
        public decimal BenefitAmount { get; set; }
    }
}