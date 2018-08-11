using JPFData;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPFinancial.Startup))]
namespace JPFinancial
{
    public partial class Startup
    {
        private readonly DatabaseEditor _dbEditor = new DatabaseEditor();

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _dbEditor.UpdateRequiredBalance();
            _dbEditor.UpdateRequiredBalanceSurplus();
            _dbEditor.UpdateBillDueDates();
        }
    }
}
