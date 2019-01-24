using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Metrics
{
    public class TimePeriodFinancialMetrics
    {
        public TimePeriodFinancialMetrics()
        {

        }

        [DataType(DataType.Currency)]
        public decimal OneMonthSavings { get; set; }

        [DataType(DataType.Currency)]
        public decimal ThreeMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public decimal SixMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public decimal OneYearSavings { get; set; }

        [Display(Name = "Monthly Expenses"), DataType(DataType.Currency)]
        public decimal MonthlyExpenses { get; set; }

        [Display(Name = "Monthly Income"), DataType(DataType.Currency)]
        public decimal MonthlyIncome { get; set; }

        [Display(Name = "Monthly Net Savings"), DataType(DataType.Currency)]
        public decimal MonthlyNetSavings { get; set; }

        public string SelectedFVType { get; set; }

        [Display(Name = "Future Value"), DataType(DataType.Currency)]
        public decimal? AmountAtFutureDate { get; set; }

        [Display(Name = "Future Date")]
        public DateTime? DateWhenFutureAmountObtained { get; set; }
    }
}
