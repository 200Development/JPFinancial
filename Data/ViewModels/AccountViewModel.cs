using System;
using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;

namespace JPFData.ViewModels
{
    /// <summary>
    /// Container that holds Account data and business logic 
    /// </summary>
    public class AccountViewModel
    {
        private AccountManager _manager;

        public AccountViewModel()
        {
            Init();
        }


        public AccountDTO Entity { get; set; }
        public AccountDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public EventCommandEnum EventCommand { get; set; }
        public EventArgumentEnum EventArgument { get; set; }


        private void Init()
        {
            Entity = new AccountDTO();
            SearchEntity = new AccountDTO();
            _manager = new AccountManager();
        }

        public bool HandleRequest()
        {
            switch (EventArgument)
            {
                case EventArgumentEnum.Create:
                    break;
                case EventArgumentEnum.Read:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Get:
                        case EventCommandEnum.Search:
                            Entity = _manager.Get(Entity);
                            return true;
                        case EventCommandEnum.Details:
                            Entity.Account = _manager.Details(Entity);
                            return true;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case EventArgumentEnum.Update:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Edit:
                            return _manager.Edit(Entity);
                        case EventCommandEnum.Rebalance:
                            Entity = _manager.Rebalance(Entity);
                            return true;
                        case EventCommandEnum.Update:
                            Entity = _manager.Update(Entity);
                            return true;
                        case EventCommandEnum.Pool:
                            break;
                    }
                    break;
                case EventArgumentEnum.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
    }
}