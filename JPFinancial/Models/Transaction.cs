using JPFinancial.Models.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        [Display(Name = "Debit")]
        public Account TransferTo { get; set; }
        [Display(Name = "Credit")]
        public Account TransferFrom { get; set; }
        public decimal? Amount { get; set; }
    }
}