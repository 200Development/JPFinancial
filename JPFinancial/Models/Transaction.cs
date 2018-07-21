using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JPFinancial.Models.Interfaces;

namespace JPFinancial.Models
{
    public class Transaction : ITransaction
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public Transaction()
        {
            Accounts = _db.Accounts.ToList();
            CreditCards = _db.CreditCards.ToList();
        }

        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
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
        public bool UsedCreditCard { get; set; }
        public int? SelectedCreditCardAccount { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
    }
}