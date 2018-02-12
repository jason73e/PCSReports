using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PCSReports.Startup))]
namespace PCSReports
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
