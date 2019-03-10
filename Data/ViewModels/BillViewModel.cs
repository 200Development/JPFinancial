using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models.JPFinancial;

namespace JPFData.ViewModels
{
    public class BillViewModel
    {
        private BillManager _manager;


        public BillViewModel()
        {
            Init();
        }


        public BillDTO Entity { get; set; }
        public BillDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public EventCommandEnum EventCommand { get; set; }
        public EventArgumentEnum EventArgument { get; set; }


        private void Init()
        {
            Entity = new BillDTO();
            SearchEntity = new BillDTO();
            _manager = new BillManager();
        }

        public bool HandleRequest()
        {
            switch (EventArgument)
            {
                case EventArgumentEnum.Create:
                    return _manager.Create(Entity);
                case EventArgumentEnum.Read:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Get:
                        case EventCommandEnum.Search:
                            Entity = _manager.Get(Entity);
                            return true;
                        case EventCommandEnum.Details:
                            Entity.Bill = _manager.Details(Entity);
                            return true;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EventArgumentEnum.Update:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Edit:
                            return _manager.Edit(Entity);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EventArgumentEnum.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }


        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMandatory { get; set; }

        public decimal AmountDue { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        public FrequencyEnum PaymentFrequency { get; set; }

        public int AccountId { get; set; }

        public IList<Account> Accounts { get; set; }

        public Account Account { get; set; }
    }
}