using System.Collections.Generic;
using JPFData.DTO;
using JPFData.Managers;

namespace JPFData.ViewModels
{
    public class AccountViewModel
    {

        public AccountViewModel()
        {
            Init();
        }


        public AccountDTO Entity { get; set; }
        public AccountDTO SearchEntity { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }
        public string Mode { get; set; }
        public bool IsValid { get; set; }
        public string EventCommand { get; set; }
        public string EventArgument { get; set; }

        private void Init()
        {
            Entity = new AccountDTO();
            SearchEntity = new AccountDTO();
        }

        public void HandleRequest()
        {
            Get();
        }

        private void Get()
        {
            AccountManager mgr = new AccountManager();

            Entity = mgr.Get(SearchEntity);
        }
        //public AccountViewModel()
        //{
        //    CurrentBalance = decimal.Zero;
        //    PaycheckContribution = decimal.Zero;
        //}

        //[Key] public int Id { get; set; }

        //[Required, StringLength(255)] public string Name { get; set; }

        //[Required, DataType(DataType.Currency)]
        //public decimal CurrentBalance { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Paycheck Contribution")]
        //public decimal? PaycheckContribution { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Required Savings")]
        //public decimal? RequiredSavings { get; set; }

        //[DataType(DataType.Currency), Display(Name = "Surplus/Deficit")]
        //public decimal? BalanceSurplus { get; set; }

        //public string BalanceFontColor { get; set; }

        //public string SurplusFontColor { get; set; }
    }
}