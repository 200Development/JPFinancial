using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    public class Transaction : ITransaction
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
        public int? CreditAccountId { get; set; }
        public int? DebitAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}