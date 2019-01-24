using System.Collections.Generic;
using JPFData.Metrics;
using JPFData.Models;

namespace JPFData.DTO
{
    public class TransactionDTO
    {
        public TransactionDTO()
        {
            Transactions = new List<Transaction>();
            Metrics = new TransactionMetrics();
        }

        public List<Transaction> Transactions { get; set; }
        public TransactionMetrics Metrics { get; set; }
    }
}