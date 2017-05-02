using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using WithoutPath.Areas.Admin;
using WithoutPath.Areas.Default;
using WithoutPath.Areas.Map;
using System.Threading.Tasks;
using System.Threading;
using WithoutPath.Hubs;
using WithoutPath.EVEAPI;
using WithoutPath.DAL;
using WithoutPath.BL;

namespace WithoutPath
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var adminArea = new AdminAreaRegistration();
            var adminAreaContext = new AreaRegistrationContext(adminArea.AreaName, RouteTable.Routes);
            adminArea.RegisterArea(adminAreaContext);

            var mapArea = new MapAreaRegistration();
            var mapAreaContext = new AreaRegistrationContext(mapArea.AreaName, RouteTable.Routes);
            mapArea.RegisterArea(mapAreaContext);

            var defaultArea = new DefaultAreaRegistration();
            var defaultAreaContext = new AreaRegistrationContext(defaultArea.AreaName, RouteTable.Routes);
            defaultArea.RegisterArea(defaultAreaContext);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Рассылка актуальной версии карты
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    MapHub.BroadcastSystems();
                    Thread.Sleep(10000);
                }
            });
        }
    }
}
