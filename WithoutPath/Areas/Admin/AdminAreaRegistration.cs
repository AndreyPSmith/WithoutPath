﻿using System.Web.Mvc;

namespace WithoutPath.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "Admin",
                url: "admin/{controller}/{action}/{page}",
                defaults: new { controller = "Posts", action = "Index", page = UrlParameter.Optional },
                namespaces: new[] { "WithoutPath.Areas.Admin.Controllers" }
            );
        }
    }
}