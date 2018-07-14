using System.ComponentModel.DataAnnotations;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    public class CreditCard : IAccount
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        [Display(Name = "Current Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Statement Balance")]
        public decimal StatementBalance { get; set; }

        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }

        [Display(Name = "Available Credit")]
        public decimal AvailableCredit { get; set; }

        [Display(Name = "Minimum Payment Due")]
        public decimal MinimumPaymentDue { get; set; }

        [Display(Name = "Purchase APR")]
        public decimal PurchaseApr { get; set; }

        [Display(Name = "Cash Advance APR")]
        public decimal CashAdvanceApr { get; set; }

        public int GracePeriodDays { get; set; }

        public int EndOfCycleDay { get; set; }
    }
}