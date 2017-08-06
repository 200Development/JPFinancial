using System;
using System.ComponentModel.DataAnnotations;

namespace Financial_Tracking.ViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Name of Account")]
        public virtual string AccountName { get; set; }

        [Display(Name = "Net Income")]
        public virtual decimal NetIncome { get; set; }

        [Display(Name = "Expense")]
        public virtual string BillName { get; set; }

        [Display(Name = "Account Balance"), DataType(DataType.Currency)]
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
    }
}