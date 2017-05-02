using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WithoutPath.Areas.Default.Controllers
{
    public class PostController : DefaultController
    {
        [HttpGet]
        public ActionResult Index(int Id = 0)
        {
            var post = Repository.Posts.FirstOrDefault(x => x.Id == Id && x.IsVerified);

            ViewBag.MainID = -1;

            if(User.Identity.IsAuthenticated && !(CurrentUser.Banned.HasValue && CurrentUser.Banned.Value))
            {
                var Main = CurrentUser.Characters.FirstOrDefault(x => !x.IsDeleted && x.IsMain.HasValue && x.IsMain.Value);
                if(Main != null)
                    ViewBag.MainID = Main.EveID;
            }


            ViewBag.IsAdmin = User.IsInRole("admin")? "True" : "False";

            if (post == null ||
               (post.IsInternal && !(ViewBag.MainID != -1 || ViewBag.IsAdmin)))
                return RedirectToNotFoundPage;

            return View(post);
        }
    }
}