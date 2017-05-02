using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WithoutPath.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class SpaceSystemsController : AdminController
    {
        // GET: Admin/SpaceSystems
        public ActionResult Index()
        {
            return View();
        }
    }
}