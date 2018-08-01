using System.Collections.Generic;
using System.Linq;
using Base;
using JPFData.Managers;
using JPFData.Models;
using BaseViewModel = JPFData.Base.BaseViewModel;

namespace JPFData.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        private AccountsManager _mgr;

        public AccountViewModel()
        {
        }


        public List<Account> Accounts { get; set; }
        public Account SearchEntity { get; set; }
        public Account Entity { get; set; }
        public string BalanceFontColor { get; set; }
        public string SurplusFontColor { get; set; }


        protected override void Init()
        {
            _mgr = new AccountsManager();
            Accounts = new List<Account>();
            SearchEntity = new Account();
            Entity = new Account();

            base.Init();
        }

        protected override void Get()
        {
            Accounts = _mgr.Get(SearchEntity);

            base.Get();
        }
    }
}