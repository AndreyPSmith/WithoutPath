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
    public class StaticsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        StaticsController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Static> Get()
        {
            return Repository.Statics;
        }

        [EnableQuery]
        public SingleResult<Static> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Statics.Where(p => p.Id == key));
        }

        [System.Web.Http.Authorize(Roles = "admin")]
        public IHttpActionResult Post(Static Static)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateStatic(Static);
            return Created(Static);
        }

        [System.Web.Http.Authorize(Roles = "admin")]
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.Statics.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveStatic(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}