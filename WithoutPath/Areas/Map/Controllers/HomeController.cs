using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WithoutPath.Controllers;
using WithoutPath.Attribute;

namespace WithoutPath.Areas.Map.Controllers
{
    [AuthorizeMap]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}