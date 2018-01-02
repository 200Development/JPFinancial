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
        public string Type { get; set; }
        public string Category { get; set; }
        [Display(Name = "Debit")]
        public string TransferTo { get; set; }
        [Display(Name = "Credit")]
        public string TransferFrom { get; set; }
        public decimal? Amount { get; set; }
    }
}