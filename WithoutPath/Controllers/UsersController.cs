using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.Http.OData.Query;
using WithoutPath.DAL;
using System.Web.Mvc;
using WithoutPath.Mappers;
using System.Web;

namespace WithoutPath.Controllers
{
    [System.Web.Http.Authorize(Roles = "admin")]
    public class UsersController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }
        public IMapper ModelMapper { get; set; }

        UsersController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
            ModelMapper = DependencyResolver.Current.GetService<IMapper>();
        }

        [EnableQuery]
        public IQueryable<UserProxy> Get()
        {
            return Repository.Users.Select(x =>
               new UserProxy
               {
                   ActivatedLink = x.ActivatedLink,
                   Password = null,
                   Note = x.Note,
                   ActivatedDate = x.ActivatedDate,
                   AddedDate = x.AddedDate,
                   Banned = x.Banned,
                   Characters = x.Characters,
                   Email = x.Email,
                   Id = x.Id,
                   UserRoles = x.UserRoles
               });
        }

        [EnableQuery]
        public SingleResult<UserProxy> Get([FromODataUri] int key)
        {
            IQueryable<UserProxy> result = Repository.Users.Where(p => p.Id == key).Select(x =>
               new UserProxy
               {
                   ActivatedLink = x.ActivatedLink,
                   Password = null,
                   Note = x.Note,
                   ActivatedDate = x.ActivatedDate,
                   AddedDate = x.AddedDate,
                   Banned = x.Banned,
                   Characters = x.Characters,
                   Email = x.Email,
                   Id = x.Id,
                   UserRoles = x.UserRoles
               });

            return SingleResult.Create(result);
        }

        public IHttpActionResult Put([FromODataUri] string key, UserProxy update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = Repository.UpdateUser((User)ModelMapper.Map(update, typeof(UserProxy), typeof(User)));
            if (result.IsError)
                return StatusCode(HttpStatusCode.InternalServerError);
           
            return Updated(update);
        }

        public  IHttpActionResult Post(UserProxy user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = Repository.CreateUser((User)ModelMapper.Map(user, typeof(UserProxy), typeof(User)));

            if(result.IsError)
                return StatusCode(HttpStatusCode.InternalServerError);

            return Created(user);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var user = Repository.Users.FirstOrDefault(x => x.Id == key);
            if (user == null)
            {
                return NotFound();
            }
            var result = Repository.RemoveUser(key);
            if (result.IsError)
                return StatusCode(HttpStatusCode.InternalServerError);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
