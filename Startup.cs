using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VinaShoseShop.Startup))]

namespace VinaShoseShop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();  // Correct way to register SignalR
        }
    }
}

