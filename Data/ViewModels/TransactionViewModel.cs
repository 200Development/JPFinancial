using System;
using System.Collections.Generic;
using System.Globalization;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Metrics;
using JPFData.Models.JPFinancial;

namespace JPFData.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
            Transaction = new Transaction();
            Transactions = new List<Transaction>();
            Metrics = new TransactionMetrics();
            AutoTransferPaycheckContributions = false;
            Date = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);

            try
            {
                Accounts = new AccountManager().GetAllAccounts();
                BillsOutstanding = new BillManager().GetOutstandingBills();
                FilterOptions = GetFilterOptions();
            }
            catch (Exception e)
            {
                Accounts = new AccountManager().GetAllAccounts();
                BillsOutstanding = new BillManager().GetOutstandingBills();
                Logger.Instance.Error(e);
            }
        }

        public Transaction Transaction { get; set; }
        public TransactionMetrics Metrics { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<OutstandingExpense> BillsOutstanding { get; set; }
        public IEnumerable<TransactionFilterOptions> FilterOptions { get; set; }
        public bool AutoTransferPaycheckContributions { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public string Date { get; set; }
        public bool IsBill { get; set; }
        public bool moreTransactions { get; set; }
        
        
        // Using Class instead of Enum to allow custom display names
        private List<TransactionFilterOptions> GetFilterOptions()
        {
            var options = new List<TransactionFilterOptions>();
            options.Add(new TransactionFilterOptions { Name = "All", DisplayName = "All Transactions" });
            options.Add(new TransactionFilterOptions { Name = "Income", DisplayName = "Income" });
            options.Add(new TransactionFilterOptions { Name = "Expense", DisplayName = "Expenses" });
            options.Add(new TransactionFilterOptions { Name = "Transfers", DisplayName = "Transfers" });
            options.Add(new TransactionFilterOptions { Name = "Rebalancing", DisplayName = "Rebalancing Transactions" });
            options.Add(new TransactionFilterOptions { Name = "Paycheck", DisplayName = "Paycheck Contributions" });


            return options;
        }
    }

    public class TransactionFilterOptions
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}