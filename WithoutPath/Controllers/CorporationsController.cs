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
    public class CorporationsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        CorporationsController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Corporation> Get()
        {
            return Repository.Corporations;
        }

        [EnableQuery]
        public SingleResult<Corporation> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Corporations.Where(p => p.Id == key));
        }

        public IHttpActionResult Post(Corporation role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateCorporation(role);
            return Created(role);
        }

        public IHttpActionResult Put([FromODataUri] string key, Corporation update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.UpdateCorporation(update);
            return Updated(update);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.Corporations.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveCorporation(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}