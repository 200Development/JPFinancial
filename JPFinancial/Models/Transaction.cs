
using JPFinancial.Models.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JPFinancial.Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public decimal? Spend { get; set; }
        public decimal? Receive { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Balance { get; set; }
        public TransactionTypesEnum TypesEnum { get; set; }
        [JsonProperty(PropertyName = "Category")]
        public IncomeCategoriesEnum IncomeCategoryEnum { get; set; }
        public List<Account> DebitAccounts { get; set; }
        public List<Account> CreditAccounts { get; set; }
        public Account SelectedDebitAccount { get; set; }
        public Account SelectedCreditAccount { get; set; }
    }
}