using System;
using JPFData.Enumerations;
using JPFData.Models.JPFinancial;

namespace JPFData.Interfaces
{
    public interface ITransaction
    {
        int Id { get; set; }
        string UserId { get; set; }
        string Payee { get; set; }
        string Memo { get; set; }
        TransactionTypesEnum Type { get; set; }
        CategoriesEnum Category { get; set; }
        int? CreditAccountId { get; set; }
        int? DebitAccountId { get; set; }
        int? SelectedExpenseId { get; set; }
        DateTime Date { get; set; }
        Account CreditAccount { get; set; }
        Account DebitAccount { get; set; }
        decimal Amount { get; set; }
        bool UsedCreditCard { get; set; }
    }
}