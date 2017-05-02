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
using Microsoft.AspNet.Identity;
using System;
using WithoutPath.DTO;

namespace WithoutPath.Controllers
{
    public class CommentsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();

        public IRepository Repository { get; set; }

        public IMapper ModelMapper { get; set; }

        CommentsController()
        {
            Repository = DependencyResolver.Current.GetService<IRepository>();
            ModelMapper = DependencyResolver.Current.GetService<IMapper>();
        }

        [EnableQuery]
        public IQueryable<CommentModel> Get()
        {
            return Repository.Comments.Select(x => new CommentModel
            {
                Id = x.Id,
                AddedDate = x.AddedDate,
                CharacterID = x.CharacterID,
                Content = x.Content,
                PostID = x.PostID,
                Character = new CharacterModel
                {
                    Id = x.Character.Id,
                    EveID = x.Character.EveID,
                    Name = x.Character.Name
                }
            });
        }

        [EnableQuery]
        public SingleResult<CommentModel> Get([FromODataUri] int key)
        {
            return SingleResult.Create(Repository.Comments.Where(x => x.Id == key).Select(x => new CommentModel {
                Id = x.Id,
                AddedDate = x.AddedDate,
                CharacterID = x.CharacterID,
                Content = x.Content,
                PostID = x.PostID,
                Character = new CharacterModel
                {
                    Id = x.Character.Id,
                    EveID = x.Character.EveID,
                    Name = x.Character.Name
                }
            }));
        }

        [AuthorizeMap]
        public IHttpActionResult Post(CommentModel instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = (Comment)ModelMapper.Map(instance, typeof(CommentModel), typeof(Comment));
            var user = Repository.GetUser(User.Identity.GetUserId());
            comment.CharacterID = user.Characters.FirstOrDefault(x => !x.IsDeleted && x.IsMain.Value).Id;

            Repository.CreateComment(comment);
            return Created(instance);
        }

        [AuthorizeMap]
        public IHttpActionResult Put([FromODataUri] string key, CommentModel update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = Repository.GetUser(User.Identity.GetUserId());
            var comment = Repository.Comments.FirstOrDefault(x => x.Id == update.Id);
            if (user != null && comment != null && (user.Id == comment.Character.UserID || User.IsInRole("admin")))
            {
                comment.Content = update.Content;
                Repository.UpdateComment(comment);
                return Updated(update);
            }

            return StatusCode(HttpStatusCode.BadRequest);
        }

        [AuthorizeMap]
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            var user = Repository.GetUser(User.Identity.GetUserId());
            var instance = Repository.Comments.FirstOrDefault(x => x.Id == key);
            if (instance == null)
            {
                return NotFound();
            }
            else if (user != null && (user.Id == instance.Character.UserID || User.IsInRole("admin")))
            {
                Repository.RemoveComment(key);
                return StatusCode(HttpStatusCode.NoContent);
            }
            return StatusCode(HttpStatusCode.BadRequest);
        }
    }
}