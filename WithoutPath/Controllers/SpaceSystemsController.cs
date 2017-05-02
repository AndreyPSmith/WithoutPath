using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.Http.OData.Query;
using WithoutPath.DAL;
using System.Web.Mvc;
using WithoutPath.Mappers;
using WithoutPath.Attribute;

namespace WithoutPath.Controllers
{
    [AuthorizeMap]
    public class SpaceSystemsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        SpaceSystemsController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<SpaceSystem> Get()
        {
            return Repository.SpaceSystems;
        }

        [EnableQuery]
        public SingleResult<SpaceSystem> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.SpaceSystems.Where(p => p.Id == key));
        }

        public IHttpActionResult Post(SpaceSystem instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateSpaceSystem(instance);
            return Created(instance);
        }

        public IHttpActionResult Put([FromODataUri] string key, SpaceSystem update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.UpdateSpaceSystem(update);
            return Updated(update);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.SpaceSystems.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveSpaceSystem(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}