using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.DTO
{
    public class TimePeriodFinancialMetrics
    {
        public TimePeriodFinancialMetrics()
        {

        }

        [DataType(DataType.Currency)]
        public string OneMonthSavings { get; set; }

        [DataType(DataType.Currency)]
        public string ThreeMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public string SixMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public string OneYearSavings { get; set; }

        [Display(Name = "Monthly Expenses"), DataType(DataType.Currency)]
        public string MonthlyExpenses { get; set; }

        [Display(Name = "Monthly Income"), DataType(DataType.Currency)]
        public string MonthlyIncome { get; set; }

        [Display(Name = "Monthly Net Savings"), DataType(DataType.Currency)]
        public string MonthlyNetSavings { get; set; }

        public string SelectedFVType { get; set; }

        [Display(Name = "Future Value"), DataType(DataType.Currency)]
        public decimal? AmountAtFutureDate { get; set; }

        [Display(Name = "Future Date")]
        public DateTime? DateWhenFutureAmountObtained { get; set; }
    }
}
