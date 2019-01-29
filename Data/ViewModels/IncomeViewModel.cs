using System;
using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;

namespace JPFData.ViewModels
{
    public class IncomeViewModel
    {
        public IncomeViewModel()
        {
            Init();
        }


        public IncomeDTO Entity { get; set; }
        public IncomeDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public EventArgumentEnum EventArgument { get; set; }


        private void Init()
        {
            Entity = new IncomeDTO();
            SearchEntity = new IncomeDTO();
        }

        public void HandleRequest()
        {
            switch (EventArgument)
            {
                case EventArgumentEnum.Create:
                case EventArgumentEnum.Read:
                    Get();
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
            IncomeManager mgr = new IncomeManager();

            Entity = mgr.Get(SearchEntity);
        }
    }
}
