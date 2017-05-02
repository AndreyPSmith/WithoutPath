using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WithoutPath
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("Content/{all}");
            routes.IgnoreRoute("Scripts/{all}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
