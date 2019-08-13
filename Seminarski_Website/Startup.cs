using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Seminarski_Website.Startup))]
namespace Seminarski_Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
