using JPFinancial.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Loan
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Original Balance")]
        public decimal OriginalBalance { get; set; }

        [Display(Name = "Current Balance")]
        public decimal CurrentBalance { get; set; }

        public decimal APR { get; set; }

        [Display(Name = "Compound Interest")]
        public FrequencyEnum CompoundFrequency { get; set; }

        public decimal Payment { get; set; }

        [Display(Name = "# of Payments")]
        public int Payments { get; set; }

        [Display(Name = "Payment Frequency")]
        public FrequencyEnum PaymentFrequency { get; set; }
    }
}