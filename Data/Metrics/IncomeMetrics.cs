using System.ComponentModel.DataAnnotations;

namespace JPFData.Metrics
{
    public class IncomeMetrics
    {
        public IncomeMetrics()
        {
            
        }


        [Display(Name = "Average Weekly Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AverageWeeklyIncome { get; set; }

        [Display(Name = "Average Monthly Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal AverageMonthlyIncome { get; set; }

        [Display(Name = "Projected Annual Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal ProjectedAnnualIncome { get; set; }

        [Display(Name = "Effective Tax Rate"), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal EffectiveTaxRate { get; set; }
    }
}
