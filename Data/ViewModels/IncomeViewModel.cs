using System;
using System.Collections.Generic;
using System.Globalization;
using JPFData.DTO;
using JPFData.Enumerations;
using JPFData.Managers;

namespace JPFData.ViewModels
{
    public class IncomeViewModel
    {
        private IncomeManager _manager;

        public IncomeViewModel()
        {
            Init();
        }


        public IncomeDTO Entity { get; set; }
        public IncomeDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public EventArgumentEnum EventArgument { get; set; }
        public EventCommandEnum EventCommand { get; set; }
        public string Date { get; set; }
        public bool AutoTransferPaycheckContributions { get; set; }



        private void Init()
        {
            Entity = new IncomeDTO();
            SearchEntity = new IncomeDTO();
            ValidationErrors = new List<KeyValuePair<string, string>>();
            _manager = new IncomeManager();
            Date = DateTime.Today.ToString("d", CultureInfo.CurrentCulture);
            AutoTransferPaycheckContributions = false;
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
                            Entity = _manager.Get(Entity);
                            return true;
                        case EventCommandEnum.Details:
                            Entity.Paycheck = _manager.GetPaycheck(Entity);
                            return true;
                    }
                    break;
                case EventArgumentEnum.Update:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Get:
                            Entity.Paycheck = _manager.GetPaycheck(Entity);
                            return true;
                        case EventCommandEnum.Edit:
                            return _manager.Edit(Entity);
                    }
                    break;
                case EventArgumentEnum.Delete:
                    switch (EventCommand)
                    {
                        case EventCommandEnum.Get:
                            Entity.Paycheck = _manager.GetPaycheck(Entity);
                            return true;
                        case EventCommandEnum.Delete:
                            return _manager.Delete(Entity.Paycheck);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
    }
}
