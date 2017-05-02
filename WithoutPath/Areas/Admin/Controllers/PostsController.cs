using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WithoutPath.Attribute;
using WithoutPath.DAL;
using WithoutPath.DTO;
using WithoutPath.Global;

namespace WithoutPath.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class PostsController : AdminController
    {
        // GET: Admin/Posts
        public ActionResult Index(int page = 1, string searchString = null)
        {
            ViewBag.SearchString = searchString;

            var request = Repository.Posts;

            if (!User.IsInRole("admin"))
            {
                request = request.Where(x => x.Character.UserID == CurrentUser.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                request = SearchEngine.Search(searchString, request.OrderByDescending(p => p.AddedDate))
                                          .AsQueryable();
            }
            else
            {
                request = request.OrderByDescending(p => p.AddedDate);
            }

            return View(new PageableData<Post>(request, page, 20));
        }

        [HttpGet]
        public ActionResult Edit(int Id = 0)
        {
            if (Id == 0)
                return View(new PostModel());

            var post = Repository.Posts.FirstOrDefault(x => x.Id == Id);
            if (post != null && (post.Character.UserID == CurrentUser.Id ||
                                 User.IsInRole("admin")))
            {
                return View((PostModel)ModelMapper.Map(post, typeof(Post), typeof(PostModel)));
            }

            return RedirectToNotFoundPage;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(PostModel model)
        {
            if (model.Id != 0)
            {
                var instance = Repository.Posts.FirstOrDefault(x => x.Id == model.Id);
                if (instance != null && (instance.Character.UserID != CurrentUser.Id &&
                                     !User.IsInRole("admin")))
                {
                    return RedirectToNotFoundPage;
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = (Post)ModelMapper.Map(model, typeof(PostModel), typeof(Post));
            post.Content = Helpers.StripScript(post.Content);

            if(post.Id == 0)
            {
                post.AddedDate = DateTime.Now;
                post.CharacterID = CurrentUser.Characters.First(x => x.IsMain.HasValue && x.IsMain.Value).Id;
            }

            if (User.IsInRole("admin"))
            {
                if(post.Id == 0 || (post.Id != 0 && post.IsVerified))
                    post.IsVerified = true;
            }
            else
            {
                post.IsVerified = false;
            }

            var result = post.Id == 0 ? Repository.CreatePost(post) : Repository.UpdatePost(post);

            if(result.IsError)
            {
                ModelState.AddModelError("Error", result.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(int VerifyId, bool VerifyStatus)
        {
            var result = Repository.VerifyPost(VerifyId, VerifyStatus);
            if(!result.IsError)
                return RedirectToAction("Edit", "Posts", new { @Id = VerifyId });

            return RedirectToErrorPage;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int PostID)
        {
            var post = Repository.Posts.FirstOrDefault(x => x.Id == PostID);
            if (post != null && (post.Character.UserID == CurrentUser.Id ||
                                 User.IsInRole("admin")))
            {
                var result = Repository.RemovePost(PostID);
                if (result.IsError)
                    return RedirectToErrorPage;

                return RedirectToAction("Index");
            }

            return RedirectToErrorPage;
        }
    }
}