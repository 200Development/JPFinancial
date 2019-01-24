using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Metrics
{
    public class BillMetrics
    {
        public BillMetrics()
        {
            LargestBalance = decimal.Zero;
            SmallestBalance = decimal.Zero;
            AverageBalance = decimal.Zero;
            PercentageOfSavings = decimal.Zero;
            TotalBalance = decimal.Zero;
        }

        [Display(Name = "Largest CurrentBalance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal LargestBalance { get; set; }

        [Display(Name = "Smallest CurrentBalance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SmallestBalance { get; set; }

        [Display(Name = "Average CurrentBalance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AverageBalance { get; set; }

        [Display(Name = "% of Savings"), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal PercentageOfSavings { get; set; }

        [Display(Name = "Total Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TotalBalance { get; set; }
    }
}
