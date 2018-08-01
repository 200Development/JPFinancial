using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.Managers;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class TransactionViewModel : Transaction
    {
        private TransactionsManager _mgrTransactions;
        private AccountsManager _mgrAccounts;

        public TransactionViewModel()
        {
           
            Init();
        }

        public List<Account> Accounts { get; set; }
        public List<CreditCard> CreditCards { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Transaction SearchEntity { get; set; }
        public Transaction Entity { get; set; }


        protected override void Init()
        {
            _mgrTransactions = new TransactionsManager();
            _mgrAccounts = new AccountsManager();
            Accounts = new List<Account>();
            CreditCards = new List<CreditCard>();
            Transactions = new List<Transaction>();
            SearchEntity = new Transaction();
            Entity = new Transaction();
            Accounts = _mgrAccounts.Get();
            CreditCards = DbContext.CreditCards.ToList();

            base.Init();
        }

        public override void HandleRequest()
        {
            // Add any Transaction Model specific commands here
            switch (EventCommand.ToLower())
            {
                case "newcommand":
                    break;
            }

            base.HandleRequest();
        }

        protected override void Add()
        {
            IsValid = true;

            // Initialize Entity
            Entity = new Transaction();
            Entity.Date = DateTime.Today;
            Accounts = DbContext.Accounts.ToList();
            CreditCards = DbContext.CreditCards.ToList();

            base.Add();
        }

        protected override void Edit()
        {
            Entity = _mgrTransactions.Get(Convert.ToInt32(EventArgument));

            base.Edit();
        }

        protected override void Save()
        {
            if (Mode == "Add")
            {
                _mgrTransactions.Insert(Entity);
            }
            else
            {
                _mgrTransactions.Update(Entity);
            }

            // Set any validation errors
            ValidationErrors = _mgrTransactions.ValidationErrors;

            // Set mode based on validation errors
            base.Save();
        }

        protected override void Delete()
        {
            // Create new entity
            Entity = new Transaction();

            // Get primary key from EventArgument
            Entity.Id = Convert.ToInt32(EventArgument);

            // Call data layer to delete record
            _mgrTransactions.Delete(Entity);

            // Reload the Data
            Get();

            base.Delete();
        }

        protected override void Get()
        {
            Transactions = _mgrTransactions.Get(SearchEntity);

            base.Get();
        }
    }
}