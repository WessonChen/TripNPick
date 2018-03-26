using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TripNPick.Startup))]
namespace TripNPick
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
