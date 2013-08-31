using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using Newtonsoft.Json;
using sharp_net;
using sharp_net.Mongo;
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

            ZicLog4Net.Instance.Config(new string[] { "Grab", "System" }, new ZicGmailConfig());
            Initialization.SiteInit();
            Initialization.TaskModelInit();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore //否则会对List生成一个都是null的对象
            };
        }


    }
}