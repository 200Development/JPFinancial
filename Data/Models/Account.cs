using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Account
    {
        public Account()
        {
            Balance = decimal.Zero;
            PaycheckContribution = decimal.Zero;
        }

        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [DataType(DataType.Currency), Display(Name = "Paycheck Contribution")]
        public decimal? PaycheckContribution { get; set; }

        [DataType(DataType.Currency), Display(Name = "Required Savings")]
        public decimal? RequiredSavings { get; set; }

        [DataType(DataType.Currency), Display(Name = "Surplus/Deficit")]
        public decimal? BalanceSurplus { get; set; }
    }
}