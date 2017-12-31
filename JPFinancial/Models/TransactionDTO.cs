using JPFinancial.Models.Enumerations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPFinancial.Models
{
    // ReSharper disable once InconsistentNaming
    public class TransactionDTO
    {
        public TransactionDTO()
        {

        }

        public string Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public decimal? Spend { get; set; }
        public decimal? Receive { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Balance { get; set; }
        [Display(Name = "Type")]
        public TransactionTypesEnum TypesEnum { get; set; }
        [Display(Name = "Category")]
        public CategoriesEnum CategoriesEnum { get; set; }
        [Display(Name = "Debit Account")]
        public List<Account> DebitAccounts { get; set; }
        [Display(Name = "Credit Account")]
        public List<Account> CreditAccounts { get; set; }
        public Account SelectedDebitAccount { get; set; }
        public Account SelectedCreditAccount { get; set; }
    }
}