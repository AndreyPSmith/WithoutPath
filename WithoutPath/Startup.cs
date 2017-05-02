using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartupAttribute(typeof(WithoutPath.Startup))]
namespace WithoutPath
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalHost.Configuration.DefaultMessageBufferSize = 1;
            app.MapSignalR();
        }
    }
}
