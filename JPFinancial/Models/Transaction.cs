using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Debit")]
        public IEnumerable<Account> CreditAccounts { get; set; }
        [Display(Name = "Credit")]
        public IEnumerable<Account> DebitAccounts { get; set; }
        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
        public decimal Amount { get; set; }
    }
}