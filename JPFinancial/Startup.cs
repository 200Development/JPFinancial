using JPFData;
using JPFData.Managers;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPFinancial.Startup))]
namespace JPFinancial
{
    public partial class Startup
    {
        private readonly Calculations _calc = new Calculations();
        private BillManager billManager = new BillManager();


        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            _calc.UpdateRequiredBalanceForBills(true);
            _calc.UpdateBalanceSurplus(true);
            billManager.UpdateBillDueDates();
        }
    }
}
