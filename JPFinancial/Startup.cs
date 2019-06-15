using JPFData;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(JPFinancial.Startup))]
namespace JPFinancial
{
    public partial class Startup
    {
        private readonly Calculations _calc = new Calculations();


        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            _calc.UpdateRequiredSavings();
            _calc.UpdateBillDueDates();
        }
    }
}
