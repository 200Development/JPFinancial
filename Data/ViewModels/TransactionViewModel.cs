using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class TransactionViewModel
    {
        //private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public TransactionViewModel()
        {
            Init();
        }


        public TransactionDTO Entity { get; set; }
        public TransactionDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public EventCommandEnum EventCommand { get; set; }
        public EventArgumentEnum EventArgument { get; set; }


        private void Init()
        {
            Entity = new TransactionDTO();
            SearchEntity = new TransactionDTO();
        }

        public void HandleRequest()
        {
            switch (EventArgument)
            {
                case EventArgumentEnum.Create:
                    break;
                case EventArgumentEnum.Read:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Search:
                        case EventCommandEnum.Get:
                            Get();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EventArgumentEnum.Update:
                    break;
                case EventArgumentEnum.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Get()
        {
            TransactionManager mgr = new TransactionManager();

            Entity = mgr.Get(SearchEntity);
        }

        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public string Memo { get; set; }
        public TransactionTypesEnum Type { get; set; }
        public CategoriesEnum Category { get; set; }
        public IEnumerable<Account> Accounts { get; set; }

        [Display(Name = "Credit")]
        public int? SelectedCreditAccount { get; set; }

        [Display(Name = "Debit")]
        public int? SelectedDebitAccount { get; set; }
        public decimal Amount { get; set; }

        [Display(Name = "Credit?")]
        public bool UsedCreditCard { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }

        [Display(Name = "Credit Card")]
        public int? SelectedCreditCardAccount { get; set; }
    }
}