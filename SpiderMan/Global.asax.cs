using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using sharp_net;
using sharp_net.Mongo;
using SpiderMan.Help;
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
            //RouteTable.Routes.MapHubs(new HubConfiguration() { EnableCrossDomain = true });

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ZicLog4Net.Instance.Config(new string[] { "Grab", "System" }, new ZicGmailConfig());
            Initialization.SiteInit();
            Initialization.TaskModelInit();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore, //mongo c#的json序列化直接使用默认JsonConvert
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance));

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));
        }

    }

}