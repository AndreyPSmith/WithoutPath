using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using WithoutPath.DAL;
using WithoutPath.DTO;

namespace WithoutPath
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("api", "api", model: GetModel());
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
        }

        public static Microsoft.OData.Edm.IEdmModel GetModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<UserProxy>("Users");
            builder.EntitySet<Role>("Roles");
            builder.EntitySet<Character>("Characters");
            builder.EntitySet<Corporation>("Corporations");
            builder.EntitySet<SpaceSystem>("SpaceSystems");
            builder.EntitySet<Link>("Links");
            builder.EntitySet<SystemType>("SystemTypes");
            builder.EntitySet<Static>("Statics");
            builder.EntitySet<SpaceSystemStatic>("SpaceSystemStatics");
            builder.EntitySet<Post>("Posts");
            builder.EntitySet<CommentModel>("Comments");

            return builder.GetEdmModel();
        }
    }
}