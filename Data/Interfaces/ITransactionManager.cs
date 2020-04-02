using System;
using System.Collections.Generic;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;
using JPFData.ViewModels;

namespace JPFData.Interfaces
{
    public interface ITransactionManager
    {
        List<Transaction> GetAllTransactions();
        Transaction GetTransaction(int? id);
        bool Create(TransactionViewModel entity);
        bool Edit(Transaction transaction);
        bool Delete(int id);


        bool HandlePaycheckContributions(Transaction transaction);
        TransactionMetrics GetMetrics();
        IEnumerable<Transaction> GetTransactionsBetweenDates(DateTime begin, DateTime end);
    }
}