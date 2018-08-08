using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JPFData.Enumerations;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class TransactionViewModel
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public TransactionViewModel()
        {
            Date = DateTime.Today;
            Accounts = _db.Accounts.ToList();
            CreditCards = _db.CreditCards.ToList();
        }

        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public IEnumerable<Account> Accounts { get; set; }

        [Display(Name = "Credit")]
        public int? SelectedCreditAccount { get; set; }

        [Display(Name = "Debit")]
        public int? SelectedDebitAccount { get; set; }
        public decimal Amount { get; set; }

        [Display(Name = "Credit?")]
        public bool UsedCreditCard { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }

        [Display(Name = "Credit Card")]
        public int? SelectedCreditCardAccount { get; set; }
    }
}