using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Models;

namespace JPFData.ViewModels
{
    public class TransactionViewModel
    {
        private TransactionManager _manager;


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
        // Type needs to be in VM or javascript will break.  todo: research this
        public TransactionTypesEnum Type { get; set; }
        public string Date { get; set; }
        public bool UsedCreditCard { get; set; }


        private void Init()
        {
            Entity = new TransactionDTO();
            SearchEntity = new TransactionDTO();
            ValidationErrors = new List<KeyValuePair<string, string>>();
            _manager = new TransactionManager();
            Date = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);
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
                        case EventCommandEnum.Search:
                        case EventCommandEnum.Get:
                            Entity = _manager.Get(SearchEntity);
                            return true;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EventArgumentEnum.Update:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Get:
                            Entity.Transaction = _manager.GetTransaction(Entity);
                            break;
                        case EventCommandEnum.Edit:
                            return _manager.Edit(Entity);
                        case EventCommandEnum.Rebalance:
                            break;
                        case EventCommandEnum.Pool:
                            break;
                        case EventCommandEnum.Update:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EventArgumentEnum.Delete:

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

    }
}