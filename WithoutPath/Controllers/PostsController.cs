using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.Http.OData.Query;
using WithoutPath.DAL;
using System.Web.Mvc;
using WithoutPath.Mappers;
using Microsoft.AspNet.Identity;

namespace WithoutPath.Controllers
{
    public class PostsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        PostsController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Post> Get()
        {
            var query = Repository.Posts.Where(x => x.IsVerified);
            bool CanSeeInternal = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = Repository.GetUser(User.Identity.GetUserId());
                if (!(user.Banned.HasValue && user.Banned.Value) && user.Characters.Any(x => !x.IsDeleted))
                    CanSeeInternal = true;
            }

            if (!CanSeeInternal)
                query = query.Where(x => !x.IsInternal);

            return query;
        }

        [EnableQuery]
        public SingleResult<Post> Get([FromODataUri] int key)
        {
            var query = Repository.Posts.Where(x => x.IsVerified);
            bool CanSeeInternal = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = Repository.GetUser(User.Identity.GetUserId());
                if (!(user.Banned.HasValue && user.Banned.Value) && user.Characters.Any(x => !x.IsDeleted))
                    CanSeeInternal = true;
            }

            if (!CanSeeInternal)
                query = query.Where(x => !x.IsInternal);

            return SingleResult.Create(query.Where(p => p.Id == key));
        }
    }
}