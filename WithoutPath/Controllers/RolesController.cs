using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.Http.OData.Query;
using WithoutPath.DAL;
using System.Web.Mvc;
using WithoutPath.Mappers;

namespace WithoutPath.Controllers
{
    [System.Web.Http.Authorize(Roles = "admin")]
    public class RolesController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        RolesController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Role> Get()
        {
            return Repository.Roles;
        }

        [EnableQuery]
        public SingleResult<Role> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Roles.Where(p => p.Id == key));
        }

        public IHttpActionResult Post(Role instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateRole(instance);
            return Created(instance);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.Roles.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveRole(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}