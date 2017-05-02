using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WithoutPath.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class CorporationsController : AdminController
    {
        // GET: Admin/Corporations
        public ActionResult Index()
        {
           
            return View();
        }
    }
}