﻿using System.ComponentModel.DataAnnotations;

namespace JPFData.Metrics
{
    public class AccountMetrics
    {
        public AccountMetrics()
        {
            LargestBalance = decimal.Zero;
            SmallestBalance = decimal.Zero;
            AverageBalance = decimal.Zero;
            PercentageOfSavings = decimal.Zero;
            LargestSurplus = decimal.Zero;
            SmallestSurplus = decimal.Zero;
            AverageSurplus = decimal.Zero;
            TotalBalance = decimal.Zero;
        }


        [Display(Name = "Largest Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal LargestBalance { get; set; }

        [Display(Name = "Smallest Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SmallestBalance { get; set; }

        [Display(Name = "Average Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AverageBalance { get; set; }

        [Display(Name = "% of Savings"), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal PercentageOfSavings { get; set; }

        [Display(Name = "Largest Surplus"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal LargestSurplus { get; set; }

        [Display(Name = "Smallest Surplus"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SmallestSurplus { get; set; }

        [Display(Name = "Average Surplus"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AverageSurplus { get; set; }

        [Display(Name = "Total Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TotalBalance { get; set; }
    }
}