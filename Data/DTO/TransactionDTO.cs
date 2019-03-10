using System;
using System.Collections.Generic;
using JPFData.Managers;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.DTO
{
    public class TransactionDTO
    {
        public TransactionDTO()
        {
            Transaction = new Transaction();
            Transactions = new List<Transaction>();
            Metrics = new TransactionMetrics();
            try
            {
                Accounts = new AccountManager().Get(new AccountDTO()).Accounts;
                CreditCards = new CreditCardManager().Get(new CreditCardDTO()).CreditCards;
                BillsOutstanding = new ExpenseManager().GetOutstandingBills();
            }
            catch (Exception)
            {
                Accounts = new List<Account>();
                CreditCards = new List<CreditCard>();
            }
        }

        public TransactionMetrics Metrics { get; set; }
        public Transaction Transaction { get; set; }
        public List<Transaction> Transactions { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
        public IEnumerable<OutstandingExpense> BillsOutstanding { get; set; }
    }
}