using System.Linq;
using Microsoft.AspNet.Identity;
using WithoutPath.BL;
using System.Web.Mvc;

namespace WithoutPath.Attribute
{
    public class AuthorizeMapHabAttribute : Microsoft.AspNet.SignalR.AuthorizeAttribute
    {
        protected override bool UserAuthorized(System.Security.Principal.IPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            var Logic = DependencyResolver.Current.GetService<ILogic>();
            var user = Logic.GetUser(principal.Identity.GetUserId());
            return user != null && 
                   !(user.Banned.HasValue && user.Banned.Value) && 
                   user.Characters.Any(x => !x.IsDeleted);
        }
    }
}