using System;
using System.ComponentModel.DataAnnotations;

namespace JPFData.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        [DataType(DataType.Currency), Display(Name = "Current Balance")]
        public decimal CurrentBalance { get; set; }

        [DataType(DataType.Currency), Display(Name = "Remaining Statement Balance")]
        public decimal RemainingStatementBalance { get; set; }

        [DataType(DataType.Currency), Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }

        [Display(Name = "Purchase APR")]
        public decimal PurchaseApr { get; set; }

        [Display(Name = "Cash Advance APR")]
        public decimal CashAdvanceApr { get; set; }

        public int GracePeriodDays { get; set; }

        [Display(Name = "Due Day Each Bill")]
        public int EndOfCycleDay { get; set; }

        [Display(Name = "Next Payment Due")]
        public DateTime NextPaymentDue { get; set; }

        [DataType(DataType.Currency), Display(Name = "Cash Advance Balance")]
        public decimal? CashAdvanceBalance  { get; set; }

        [DataType(DataType.Currency), Display(Name = "Cash Advance Limit")]
        public decimal? CashAdvanceLimit { get; set; }
    }
}