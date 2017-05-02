using System.Web.Mvc;

namespace WithoutPath.Areas.Map
{
    public class MapAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Map";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               name: "Map",
               url: "Map/{controller}/{action}",
               defaults: new { controller = "Home", action = "Index"},
               namespaces: new[] { "WithoutPath.Areas.Map.Controllers" }
           );
        }
    }
}