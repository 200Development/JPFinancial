using System;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Transaction
    {
        public Transaction()
        {
            UserId = Global.Instance.User == null ? string.Empty : Global.Instance.User.Id;
            Date = DateTime.Today;
            Payee = string.Empty;
            Memo = string.Empty;
            CreditAccountId = 0;
            DebitAccountId = 0;
            SelectedExpenseId = 0;
            Amount = 0.0m;;
        }

        [Required, Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required, StringLength(255)]
        public string Payee { get; set; }

        [StringLength(255)]
        public string Memo { get; set; }

        [Required]
        public TransactionTypesEnum Type { get; set; }

        [Required]
        public CategoriesEnum Category { get; set; }

        public int? CreditAccountId { get; set; }
        public int? DebitAccountId { get; set; }
        public int? SelectedExpenseId { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "From")]
        public Account CreditAccount { get; set; }

        [Display(Name = "To")]
        public Account DebitAccount { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required, Display(Name = "Charged to Credit Card?")]
        public bool UsedCreditCard { get; set; }
    }
}