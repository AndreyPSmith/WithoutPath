﻿using System.Data;
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
    public class LinksController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        LinksController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
        }

        [EnableQuery]
        public IQueryable<Link> Get()
        {
            return Repository.Links;
        }

        [EnableQuery]
        public SingleResult<Link> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Links.Where(p => p.Id == key));
        }

        public IHttpActionResult Post(Link instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.CreateLink(instance);
            return Created(instance);
        }

        public IHttpActionResult Put([FromODataUri] string key, Link update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Repository.UpdateLink(update);
            return Updated(update);
        }

        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var instance = Repository.Links.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            Repository.RemoveLink(key);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}