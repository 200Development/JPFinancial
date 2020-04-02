using System.Collections.Generic;
using JPFData.Enumerations;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using PagedList;

namespace JPFData.Interfaces
{
    public interface ITransactionViewModel
    {
        Transaction Transaction { get; set; }
        TransactionMetrics Metrics { get; set; }
        IEnumerable<Transaction> Transactions { get; set; }
        IPagedList<Transaction> PagedTransactions { get; set; }
        IEnumerable<Account> Accounts { get; set; }
        bool AutoTransferPaycheckContributions { get; }
        TransactionTypesEnum Type { get; set; }
        CategoriesEnum Category { get; set; }
        string Date { get; set; }
        bool IsBill { get; set; }
    }
}