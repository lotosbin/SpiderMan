using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(SpiderMan.Startup))]
namespace SpiderMan {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            //var config = new HubConfiguration();
            ////代替 RouteTable.Routes.MapHubs(new HubConfiguration() { EnableCrossDomain = true });
            //config.EnableJSONP = true;
            //app.MapSignalR(config);

            app.Map("/signalr", map => { //http://goo.gl/StJL4y
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration {
                    // JSONP 请求不安全，但一些老浏览器必须使用JSONP实现cross domain
                    EnableJSONP = true
                };
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}