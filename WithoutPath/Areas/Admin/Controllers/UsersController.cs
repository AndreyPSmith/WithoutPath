using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WithoutPath.DAL;
using WithoutPath.DTO;

namespace WithoutPath.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : AdminController
    {
        // GET: Admin/Users
        public ActionResult Index()
        {         
            return View();
        }
    }
}