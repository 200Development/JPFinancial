using JPFinancial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace JPFinancial.ViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Name of AccountId")]
        public virtual string AccountName { get; set; }

        [Display(Name = "Net Income")]
        public virtual decimal NetIncome { get; set; }

        [Display(Name = "Expense")]
        public virtual string BillName { get; set; }

        [Display(Name = "AccountId Balance"), DataType(DataType.Currency)]
        public virtual decimal AccountBalance { get; set; }

        [Display(Name = "Amount Due"), DataType(DataType.Currency)]
        public virtual decimal DueAmount { get; set; }

        [Display(Name = "Due Date")]
        public virtual DateTime DueDate { get; set; }

        [Display(Name = "Future Value"), DataType(DataType.Currency)]
        public virtual decimal? FutureAmount { get; set; }

        [Display(Name = "Future Date")]
        public virtual DateTime? FutureDate { get; set; }

        [Display(Name = "Is Mandatory")]
        public virtual bool IsMandatory { get; set; }

        public string SelectedFVType { get; set; }

        [DataType(DataType.Currency)]
        public virtual string OneMonthSavings { get; set; }

        [DataType(DataType.Currency)]
        public virtual string ThreeMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public virtual string SixMonthsSavings { get; set; }

        [DataType(DataType.Currency)]
        public virtual string OneYearSavings { get; set; }

        [Display(Name = "Monthly Expenses"), DataType(DataType.Currency)]
        public virtual string MonthlyExpenses { get; set; }

        [Display(Name = "Monthly Income"), DataType(DataType.Currency)]
        public virtual string MonthlyIncome { get; set; }

        [Display(Name = "Monthly Net Savings"), DataType(DataType.Currency)]
        public virtual string MonthlyNetSavings { get; set; }

        [Display(Name = "Savings %"), DataType(DataType.Currency)]
        public virtual string SavingsPercentage { get; set; }

        [Display(Name = "Net Worth"), DataType(DataType.Currency)]
        public virtual string NetWorth { get; set; }

        public virtual string SavedUp { get; set; }

        public virtual string TotalDue { get; set; }
        public virtual string TotalMonthlyDue { get; set; }
        public virtual string BillsDue { get; set; }
        public virtual List<Account> Accounts { get; set; }
        public virtual string CurrentMonth { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<Transaction> TopTransactions { get; set; }
        public List<Dictionary<DateTime, LoanViewModel>> LoanViewModelByMonth { get; set; }
        public Chart AccountGraph { get; set; }
    }
}