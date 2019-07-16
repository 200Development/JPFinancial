using System.ComponentModel.DataAnnotations;

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
            CashBalance = decimal.Zero;
            OutstandingExpenses = decimal.Zero;
            PoolBalance = decimal.Zero;
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

        [Display(Name = "Cash Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal CashBalance { get; set; }

        [Display(Name = "Accounting Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AccountingBalance { get; set; }

        [Display(Name = "Spendable Cash"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SpendableCash { get; set; }

        [Display(Name = "Monthly Surplus"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal MonthlySurplus { get; set; }

        [Display(Name = "Outstanding Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal OutstandingExpenses { get; set; }

        [Display(Name = "Pool"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal PoolBalance { get; set; }
    }
}
