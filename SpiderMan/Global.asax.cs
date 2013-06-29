using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using sharp_net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SpiderMan {

    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            RouteTable.Routes.MapHubs(new HubConfiguration() { EnableCrossDomain = true });

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ZicLog4Net.Instance.Config(new string[] { "Grab", "System" }, new ZicGmailConfig());
        }
    }
}