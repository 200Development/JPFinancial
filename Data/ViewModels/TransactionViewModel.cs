using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;

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
        public EventCommandEnum EventCommand { get; set; }
        public EventArgumentEnum EventArgument { get; set; }
        // Type needs to be in VM or javascript will break.  Would normally put in the DTO (https://www.codeproject.com/articles/1050468/data-transfer-object-design-pattern-in-csharp)  todo: research this
        public TransactionTypesEnum Type { get; set; }
        public string Date { get; set; }
        public bool AutoTransferPaycheckContributions { get; set; }

        //TODO: research method to move this to Transaction Class.  Javascript won't fire when referencing from Transaction
        [Display(Name = "Charged to Credit Card?")]
        public bool UsedCreditCard { get; set; }
        public bool IsBill { get; set; }


        private void Init()
        {
            Entity = new TransactionDTO();
            SearchEntity = new TransactionDTO();
            ValidationErrors = new List<KeyValuePair<string, string>>();
            _manager = new TransactionManager();
            Date = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);
            AutoTransferPaycheckContributions = false;
        }

        public bool HandleRequest()
        {
            Logger.Instance.DataFlow($"ViewModel Handle Request.  EventArgument: {EventArgument.ToString()}, EventCommand: {EventCommand.ToString()}");
            switch (EventArgument)
            {
                case EventArgumentEnum.Create:
                    if (!AutoTransferPaycheckContributions) return _manager.Create(Entity);
                    if(!_manager.HandlePaycheckContributions(Entity.Transaction)) return false;
                    return _manager.Create(Entity);
                case EventArgumentEnum.Read:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Search:
                        case EventCommandEnum.Get:
                            Entity = _manager.Get(Entity);
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
                    return _manager.Delete(Entity.Transaction);

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

    }
}