using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class Account
    {
        public Account()
        {
            Balance = decimal.Zero;
            PaycheckContribution = decimal.Zero;
            BalanceSurplus = decimal.Zero;
            SuggestedPaycheckContribution = decimal.Zero;
        }

        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [DataType(DataType.Currency), Display(Name = "Paycheck Contribution")]
        public decimal? PaycheckContribution { get; set; }

        [DataType(DataType.Currency), Display(Name = "Suggested Paycheck Contribution")]
        public decimal? SuggestedPaycheckContribution { get; set; }

        [DataType(DataType.Currency), Display(Name = "Required Savings")]
        public decimal? RequiredSavings { get; set; }

        [DataType(DataType.Currency), Display(Name = "Surplus/Deficit")]
        public decimal? BalanceSurplus { get; set; }

        [Display(Name = "Exclude From Surplus")]
        public bool ExcludeFromSurplus { get; set; }

        [Display(Name = "% of Savings")]
        public decimal PercentageOfSavings { get; set; }

        [Display(Name = "Pool Account")]
        public bool IsPoolAccount { get; set; }
    }
}