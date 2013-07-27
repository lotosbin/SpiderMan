using sharp_net.Infrastructure;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SpiderMan {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());

            XDocument myXDoc = XDocument.Load(HttpContext.Current.Server.MapPath("~/htmlfilter.xml"));
            XElement ele = myXDoc.Element("config");
            FilterXmlConfig filterXmlConfig = new FilterXmlConfig(ele);
            htmlFilter = new HtmlFilter(filterXmlConfig);
        }

        public static HtmlFilter htmlFilter { get; private set; }
    }
}