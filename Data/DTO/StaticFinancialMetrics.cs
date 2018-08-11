using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.DTO
{
    public class StaticFinancialMetrics
    {
        //TODO: Need to rename many of the properties for clarity and to create more general terms
        public StaticFinancialMetrics()
        {

        }


        [Display(Name = "Net Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal NetIncome { get; set; }

        [Display(Name = "Savings %"), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal SavingsPercentage { get; set; }

        [Display(Name = "Net Worth"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal NetWorth { get; set; }

        [Display(Name = "Saved Up"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal SavedUp { get; set; }

        [Display(Name = "Total Due"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TotalDue { get; set; }

        [Display(Name = "Total Monthly Due"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TotalMonthlyDue { get; set; }

        [Display(Name = "Bills Due"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BillTotalDue { get; set; }

        [Display(Name = "Mandatory Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal MandatoryExpenses { get; set; }

        [Display(Name = "Discretionary Spending"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DiscretionarySpending { get; set; }

        [Display(Name = "Savings Rate"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal SavingsRate { get; set; }

        [Display(Name = "Costliest Category"), DataType(DataType.Currency)]
        public CategoriesEnum CostliestCategory { get; set; }

        [Display(Name = "Costliest Expense %"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal CostliestExpensePercentage { get; set; }

        [Display(Name = "Costliest Expense $"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal CostliestExpenseAmount { get; set; }

        [Display(Name = "Loan Interest % of Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal LoanInterestPercentOfIncome { get; set; }

        [Display(Name = "Monthly Loan Interest"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal MonthlyLoanInterest { get; set; }

        [Display(Name = "Daily Loan Interest %"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal DailyLoanInterestPercentage { get; set; }

        [Display(Name = "Daily Loan Interest $"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DailyLoanInterest { get; set; }

        [Display(Name = "Disposable Income %"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal DisposableIncomePercentage { get; set; }

        [Display(Name = "Disposable Income $"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DisposableIncome { get; set; }

        [Display(Name = "Discretionary Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DiscretionaryExpenses { get; set; }

        [Display(Name = "Last Month's Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal LastMonthExpenses { get; set; }

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Expenses { get; set; }
    }
}
