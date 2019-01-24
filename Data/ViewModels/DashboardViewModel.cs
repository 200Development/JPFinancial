using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Managers;

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

        public bool IsCreateTransactionVisible { get; set; }
        public bool IsTransactionDetailsVisible { get; set; }
        public bool IsTransactionListAreaVisible { get; set; }


        private void Init()
        {
            EventCommand = "List";
            EventArgument = string.Empty;
            ValidationErrors = new List<KeyValuePair<string, string>>();

            IsCreateTransactionVisible = true;
            IsTransactionDetailsVisible = false;
            IsTransactionListAreaVisible = true;

            ListMode();
        }

        private void ListMode()
        {
            IsValid = true;

            IsCreateTransactionVisible = true;
            IsTransactionDetailsVisible = false;
            IsTransactionListAreaVisible = true;

            Mode = "List";
        }

        private void Get()
        {
            DashboardManager mgr = new DashboardManager();

            Entity = mgr.Get(SearchEntity);
        }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get();
                    break;
                case "cancel":
                    ListMode();
                    Get();
                    break;
            }
        }
    }
}