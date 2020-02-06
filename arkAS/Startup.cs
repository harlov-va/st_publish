using arkAS.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(arkAS.Startup))]
namespace arkAS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
