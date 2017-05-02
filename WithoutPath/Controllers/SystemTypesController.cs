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
    [System.Web.Http.Authorize]
    public class SystemTypesController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        SystemTypesController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<SystemType> Get()
        {
            return Repository.SystemTypes;
        }

        [EnableQuery]
        public SingleResult<SystemType> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.SystemTypes.Where(p => p.Id == key));
        }

        [System.Web.Http.Authorize(Roles = "admin")]
        public IHttpActionResult Post(SystemType SystemType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateSystemType(SystemType);
            return Created(SystemType);
        }

        [System.Web.Http.Authorize(Roles = "admin")]
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.SystemTypes.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveSystemType(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}