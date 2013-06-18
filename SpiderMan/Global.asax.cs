using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using sharp_net;
using SpiderMan.CustomModelBinders;
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
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            //Encoding oldDefault = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedEncodings[0];
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedEncodings.Add(oldDefault);
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedEncodings.RemoveAt(0);

            RouteTable.Routes.MapHubs(new HubConfiguration() { EnableCrossDomain = true });

            //http://www.drdobbs.com/database/mongodb-with-c-deep-dive/240152181
            //ModelBinders.Binders.Add(typeof(ObjectId), new BsonObjectIdBinder());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            ZicLog4Net.Instance.Config(new string[] { "Grab", "System" }, new ZicGmailConfig());
        }
    }
}