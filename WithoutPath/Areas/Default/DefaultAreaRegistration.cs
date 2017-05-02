using System.Web.Mvc;

namespace WithoutPath.Areas.Default
{
    public class DefaultAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Default";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
              null,
              url: "Error",
              defaults: new { controller = "Error", action = "Index"},
              namespaces: new[] { "WithoutPath.Areas.Default.Controllers" }
          );

            context.MapRoute(
                null,
                url: "NotFoundPage",
                defaults: new { controller = "Error", action = "NotFoundPage"},
                namespaces: new[] { "WithoutPath.Areas.Default.Controllers" }
            );

            context.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{Id}",
                defaults: new { controller = "Home", action = "Index", Id = UrlParameter.Optional },
                namespaces: new[] { "WithoutPath.Areas.Default.Controllers" }
            );
        }
    }
}