using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SpiderMan.Startup))]
namespace SpiderMan {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
            //http://stackoverflow.com/questions/20068075/owin-startup-class-missing
        }
    }
}