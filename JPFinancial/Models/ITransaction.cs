using JPFinancial.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace JPFinancial.Models
{
    public interface ITransaction
    {
        int Id { get; set; }
        DateTime Date { get; set; }
        string Payee { get; set; }
        string Memo { get; set; }
        TransactionTypesEnum Type { get; set; }
        CategoriesEnum Category { get; set; }
        IEnumerable<Account> CreditAccounts { get; set; }
        IEnumerable<Account> DebitAccounts { get; set; }
        decimal Amount { get; set; }
    }
}