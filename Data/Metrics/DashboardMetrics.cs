using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Metrics
{
    public class DashboardMetrics
    {
        //TODO: Need to rename many of the properties for clarity and to create more general terms
        public DashboardMetrics()
        {
            NetIncome = 0.00m;
            TotalDue = 0.00m;
            CashBalance = 0.00m;
            CostliestExpensePercentage = 0.00m;
            CostliestExpenseAmount = 0.00m;
            LoanInterestPercentOfIncome = 0.00m;
            MonthlyLoanInterest = 0.00m;
            DailyLoanInterest = 0.00m;
            Expenses = 0.00m;
            DueBeforeNextPayPeriod = 0.00m;
            MinimumMonthlyExpenses = 0.00m;
            DisposableIncome = 0.00m;
            EmergencyFundRatio = 0.00m;
            TargetedNetWorth = 0.00m;
            CurrentRatio = 0.00m;
            DebtToIncome = 0.00m;
            SavingsRate = 0.00m;
            BudgetRuleDiscretionary = 0.00m;
            BudgetRuleExpense = 0.00m;
            BudgetRuleSavings = 0.00m;
        }


        [Display(Name = "Due Before Next Pay Period"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DueBeforeNextPayPeriod { get; set; }

        [Display(Name = "Minimum Monthly Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal MinimumMonthlyExpenses { get; set; }

        [Display(Name = "Cash Balance"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal CashBalance { get; set; }

        [Display(Name = "Disposable Income"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DisposableIncome { get; set; }

        [Display(Name = "Emergency Fund Ratio"), DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal EmergencyFundRatio { get; set; }

        [Display(Name = "Targeted Net Worth"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TargetedNetWorth { get; set; }

        [Display(Name = "Current Ratio"), DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal CurrentRatio { get; set; }

        [Display(Name = "Debt-To-Income"), DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal DebtToIncome { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BudgetRuleExpense { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BudgetRuleSavings { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BudgetRuleDiscretionary { get; set; }

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
        
        [Display(Name = "Discretionary Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal DiscretionaryExpenses { get; set; }

        [Display(Name = "Last Month's Expenses"), DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal LastMonthExpenses { get; set; }

        [DataType(DataType.Currency), DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Expenses { get; set; }

        public decimal LastMonthDiscretionarySpending { get; set; }
        public decimal LastMonthMandatoryExpenses { get; set; }
        public decimal? PercentageChangeExpenses { get; set; }
        public Dictionary<string, decimal> ExpensesByMonth { get; set; }
        public Dictionary<string, decimal> MandatoryExpensesByMonth { get; set; }
        public Dictionary<string, decimal> DiscretionarySpendingByMonth { get; set; }
        public Dictionary<string, decimal> CashFlowByMonth { get; set; }
    }
}
