using JPFinancial.Models;
using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace JPFinancial.ViewModels
{
    public class CreateTransactionViewModel : ITransaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public IEnumerable<Account> CreditAccounts { get; set; }
        public IEnumerable<Account> DebitAccounts { get; set; }
        public int? SelectedTransferToAccount { get; set; }
        public int? SelectedTransferFromAccount { get; set; }
        public decimal? Amount { get; set; }
    }
}