using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WithoutPath.BL;

namespace WithoutPath.Attribute
{
    public class AuthorizeMapAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
                return false;

            var Logic = DependencyResolver.Current.GetService<ILogic>();
            var user = Logic.GetUser(httpContext.User.Identity.GetUserId());
            return user != null &&
                   !(user.Banned.HasValue && user.Banned.Value) &&
                   user.Characters.Any(x => !x.IsDeleted);
        }
    }
}