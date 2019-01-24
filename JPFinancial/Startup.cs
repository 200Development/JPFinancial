using JPFData;
using JPFData.Managers;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPFinancial.Startup))]
namespace JPFinancial
{
    public partial class Startup
    {
        private AccountManager accountManager = new AccountManager();
        private BillManager billManager = new BillManager();


        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            accountManager.UpdateRequiredBalance();
            accountManager.UpdateRequiredBalanceSurplus();
            billManager.UpdateBillDueDates();
        }
    }
}
