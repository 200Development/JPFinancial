using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPFinancial.Startup))]
namespace JPFinancial
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
