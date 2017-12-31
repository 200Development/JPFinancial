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
        public string TransferTo { get; set; }
        public string TransferFrom { get; set; }
        public decimal? Spend { get; set; }
        public decimal? Receive { get; set; }
        public decimal? Amount { get; set; }
    }
}