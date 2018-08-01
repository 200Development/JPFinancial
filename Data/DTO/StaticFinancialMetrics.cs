using System.ComponentModel.DataAnnotations;

namespace JPFData.DTO
{
    public class StaticFinancialMetrics
    {
        public StaticFinancialMetrics()
        {

        }

        [Display(Name = "Savings %"), DataType(DataType.Currency)]
        public string SavingsPercentage { get; set; }

        [Display(Name = "Net Worth"), DataType(DataType.Currency)]
        public string NetWorth { get; set; }

        public string SavedUp { get; set; }
        public string TotalDue { get; set; }
        public string TotalMonthlyDue { get; set; }
        public string BillsDue { get; set; }
        public string MandatoryExpenses { get; set; }
        public string DiscretionarySpending { get; set; }
        public string SavingsRate { get; set; }
        public string CostliestCategory { get; set; }
        public string CostliestExpensePercentage { get; set; }
        public string CostliestExpenseAmount { get; set; }
        public string LoanInterestPercentOfIncome { get; set; }
        public string MonthlyLoanInterest { get; set; }
        public string DailyLoanInterestPercentage { get; set; }
        public string DailyLoanInterest { get; set; }
        public string DisposableIncomePercentage { get; set; }
        public string DisposableIncome { get; set; }
    }
}
