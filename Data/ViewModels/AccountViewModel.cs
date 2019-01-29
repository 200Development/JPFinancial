using System;
using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;
using JPFData.Migrations;

namespace JPFData.ViewModels
{
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

        public void HandleRequest()
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
                            Get();
                            break;
                        case EventCommandEnum.Details:
                            Details();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EventArgumentEnum.Update:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Edit:
                        case EventCommandEnum.Rebalance:
                            Rebalance();
                            break;
                        case EventCommandEnum.Update:
                            Update();
                            break;
                        case EventCommandEnum.Pool:
                            break;
                    }
                    break;
                case EventArgumentEnum.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Details()
        {
            Entity.Account = _manager.Details(Entity);
        }

        private void Get()
        {
            Entity = _manager.Get(Entity);
        }

        private void Rebalance()
        {
            Entity = _manager.Rebalance(Entity);
        }

        private void Update()
        {
            Entity = _manager.Update(Entity);
        }
    }
}