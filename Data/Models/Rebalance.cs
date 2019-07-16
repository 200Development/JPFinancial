using System;
using System.ComponentModel.DataAnnotations;
using JPFData.Enumerations;

namespace JPFData.Models.JPFinancial
{
    public class Rebalance
    {
        public Rebalance()
        {
            Date = DateTime.Today;
            Payee = string.Empty;
            Memo = string.Empty;
            CreditAccountId = 0;
            DebitAccountId = 0;
            Amount = decimal.Zero;
        }


        public int Id { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public int? CreditAccountId { get; set; }
        public int? DebitAccountId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }
}