using System;
using System.Collections.Generic;
using System.Linq;
using JPFData.DTO;
using JPFData.Managers;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            Init();
        }


        public DashboardDTO Entity { get; set; }
        public DashboardDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public string EventCommand { get; set; }
        public string EventArgument { get; set; }

        public bool IsCreateAreaVisible { get; set; }
        public bool IsDetailAreaVisible { get; set; }
        public bool IsListAreaVisible { get; set; }



        private void Init()
        {
            EventCommand = "List";
            EventArgument = string.Empty;
            ValidationErrors = new List<KeyValuePair<string, string>>();

            IsCreateAreaVisible = true;
            IsDetailAreaVisible = false;
            IsListAreaVisible = true;

            ListMode();
        }

        private void ListMode()
        {
            IsValid = true;

            IsCreateAreaVisible = true;
            IsDetailAreaVisible = false;
            IsListAreaVisible = true;

            Mode = "List";
        }

        private void AddMode()
        {
            IsCreateAreaVisible = true;
            IsDetailAreaVisible = false;
            IsListAreaVisible = true;

            Mode = "Add";
        }

        private void Add()
        {
            DashboardManager mgr = new DashboardManager();

            if (Mode == "Add" || Mode == "List")
            {
                var insertSuccess = mgr.Insert(Entity);
                if (insertSuccess)
                    Entity = mgr.Get(Entity);
            }
            else
            {
                mgr.Update(Entity);
            }


            //Reload data
            Get();

            //TODO: Need to clean up navigation, what panels show and when (shouldn't go to ListMode from Add
            ListMode();
        }

        private void EditMode()
        {
            IsCreateAreaVisible = false;
            IsDetailAreaVisible = true;
            IsListAreaVisible = true;

            Mode = "Edit";
        }

        private void Edit()
        {
            // Get Product Data
            TransactionManager transactionManager = new TransactionManager();
            var transactions = transactionManager.Get(new Transaction());
            var transaction = transactions.FirstOrDefault(t => t.Id == Convert.ToInt32(EventArgument));
            var transactionViewModel = new TransactionViewModel();

            if (transaction != null)
            {
                transactionViewModel.Id = transaction.Id;
                transactionViewModel.Accounts = transaction.Accounts;
                transactionViewModel.Amount = transaction.Amount;
                transactionViewModel.Category = transaction.Category;
                transactionViewModel.CreditCards = transaction.CreditCards;
                transactionViewModel.Date = transaction.Date;
                transactionViewModel.Memo = transaction.Memo;
                transactionViewModel.Payee = transaction.Payee;
                transactionViewModel.SelectedCreditAccount = transaction.CreditAccountId;
                transactionViewModel.SelectedDebitAccount = transaction.DebitAccountId;
                transactionViewModel.Type = transaction.Type;
                transactionViewModel.UsedCreditCard = transaction.UsedCreditCard;
            }

            Entity.CreateTransaction = transactionViewModel;
            Entity.Transactions = transactions;
            // Put View Model into Edit Mode
            EditMode();
        }

        private void Save()
        {

            if (Mode == "Add")
            {
                //mgr.Insert(Entity);
            }
            else
            {
                Entity.CreateTransaction.Id = Convert.ToInt32(EventArgument);

                DashboardManager mgr = new DashboardManager();

                mgr.Update(Entity);
            }

            // Set any validation errors
            //ValidationErrors = mgr.ValidationErrors;
            if (ValidationErrors.Count > 0)
            {
                IsValid = false;
            }

            if (!IsValid)
            {
                if (Mode == "Add")
                {
                    AddMode();
                }
                else
                {
                    EditMode();
                }
            }

            // Reload the Data
            Get();

            // Set back to normal mode
            ListMode();
        }

        private void Delete()
        {
            DashboardManager mgr = new DashboardManager();

            // Create new entity
            Entity = new DashboardDTO();

            // Get primary key from EventArgument
            var id = Convert.ToInt32(EventArgument);

            // Call data layer to delete record
            mgr.DeleteTransaction(id);

            // Reload the Data
            Get();

            // Set back to normal mode
            ListMode();
        }

        private void Get()
        {
            DashboardManager mgr = new DashboardManager();

            Entity = mgr.Get(SearchEntity);
        }

        private void Reset()
        {
            Entity.CreateTransaction = new TransactionViewModel();
        }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get();
                    break;

                case "add":
                    Add();

                    break;

                case "edit":
                    IsValid = true;
                    Edit();
                    break;

                case "delete":
                    //ResetSearch();
                    Delete();
                    break;

                case "save":
                    Save();
                    break;

                case "cancel":
                    ListMode();
                    Get();
                    break;

                case "resetadd":
                    Reset();
                    Get();
                    break;
            }
        }
    }
}