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
    public class CharactersController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        CharactersController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Character> Get()
        {
            return Repository.Characters.Where(x=> !x.IsDeleted);
        }

        [EnableQuery]
        public SingleResult<Character> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Characters.Where(p => p.IsDeleted && p.Id == key));
        }

        public IHttpActionResult Post(Character instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateCharacter(instance);
            return Created(instance);
        }

        public IHttpActionResult Put([FromODataUri] string key, Character update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.UpdateCharacter(update);
            return Updated(update);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.Characters.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveCharacter(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}