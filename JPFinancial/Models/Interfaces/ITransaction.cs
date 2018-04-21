using System;
using System.Collections.Generic;
using JPFinancial.Models.Enumerations;

namespace JPFinancial.Models.Interfaces
{
    public interface ITransaction
    {
        int Id { get; set; }
        DateTime Date { get; set; }
        string Payee { get; set; }
        string Memo { get; set; }
        TransactionTypesEnum Type { get; set; }
        CategoriesEnum Category { get; set; }
        IEnumerable<Account> Accounts { get; set; }
        decimal Amount { get; set; }
    }
}