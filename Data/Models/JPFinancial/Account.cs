﻿using System.ComponentModel.DataAnnotations;

namespace JPFData.Models.JPFinancial
{
    public class Account
    {
        public Account()
        {
            UserId = string.Empty;
            Name = string.Empty;
            Balance = decimal.Zero;
            PaycheckContribution = decimal.Zero;
            SuggestedPaycheckContribution = decimal.Zero;
            RequiredSavings = decimal.Zero;
            BalanceSurplus = decimal.Zero;
            ExcludeFromSurplus = false;
            PercentageOfSavings = decimal.Zero;
            IsPoolAccount = false;
            IsMandatory = false;
            BalanceLimit = decimal.Zero;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Paycheck Contribution")]
        public decimal PaycheckContribution { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Suggested Paycheck Contribution")]
        public decimal SuggestedPaycheckContribution { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Required Savings")]
        public decimal RequiredSavings { get; set; }

        [Required, DataType(DataType.Currency), Display(Name = "Surplus/Deficit")]
        public decimal BalanceSurplus { get; set; }

        [Required, Display(Name = "Exclude From Surplus")]
        public bool ExcludeFromSurplus { get; set; }

        [Required, Display(Name = "% of Savings")]
        public decimal PercentageOfSavings { get; set; }

        [Required, Display(Name = "Pool Account")]
        public bool IsPoolAccount { get; set; }

        [Required, Display(Name = "Mandatory Account?")]
        public bool IsMandatory { get; set; }

        [Required, Display(Name = "Balance Limit")]
        public decimal BalanceLimit { get; set; }
    }
}