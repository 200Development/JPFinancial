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
                Accounts = new AccountManager().GetAllAccounts();
                CreditCards = new CreditCardManager().Get(new CreditCardDTO()).CreditCards;
                BillsOutstanding = new BillManager().GetOutstandingBills();
                FilterOptions = GetFilterOptions();
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
        public IEnumerable<TransactionFilterOptions> FilterOptions { get; set; }

        // Using Class instead of Enum to allow custom display names
        private List<TransactionFilterOptions> GetFilterOptions()
        {
            var options = new List<TransactionFilterOptions>();
            options.Add(new TransactionFilterOptions {Name = "All", DisplayName = "All Transactions"});
            options.Add(new TransactionFilterOptions {Name = "Income", DisplayName = "Income"});
            options.Add(new TransactionFilterOptions {Name = "Expense", DisplayName = "Expenses"});
            options.Add(new TransactionFilterOptions {Name = "Transfers", DisplayName = "Transfers"});
            options.Add(new TransactionFilterOptions {Name = "Rebalancing", DisplayName = "Rebalancing Transactions"});
            options.Add(new TransactionFilterOptions {Name = "Paycheck", DisplayName = "Paycheck Contributions"});


            return options;
        }
    }

    public class TransactionFilterOptions
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

   

    //public enum TransactionFilterEnum
    //{
    //    All,
    //    Income,
    //    Expense,
    //    Transfers,
    //    Rebalancing,
    //    Paycheck
    //}
}