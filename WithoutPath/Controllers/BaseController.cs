using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WithoutPath.DAL;
using WithoutPath.BL;
using Ninject;
using WithoutPath.Global.Config;
using WithoutPath.Mappers;
using WithoutPath.EVEAPI;

namespace WithoutPath.Controllers
{
    public class BaseController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string HostName = string.Empty;

        [Inject]
        public IRepository Repository { get; set; }

        [Inject]
        public ILogic Logic { get; set; }

        [Inject]
        public IConfig Config { get; set; }

        [Inject]
        public IMapper ModelMapper { get; set; }

        [Inject]
        public IEVEProvider EVEProvider { get; set; }

        public User CurrentUser
        {
            get
            {
                return Repository.GetUser(User.Identity.GetUserId());
            }
        }

        protected static string ErrorPage = "~/Error";

        protected static string NotFoundPage = "~/NotFoundPage";

        protected static string LoginPage = "~/Account/Login";

        public RedirectResult RedirectToNotFoundPage
        {
            get
            {
                return Redirect(NotFoundPage);
            }
        }

        public RedirectResult RedirectToLoginPage
        {
            get
            {
                return Redirect(LoginPage);
            }
        }

        public RedirectResult RedirectToErrorPage
        {
            get
            {
                return Redirect(ErrorPage);
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //if (requestContext.HttpContext.Request.Url != null)
            //{
            //    HostName = requestContext.HttpContext.Request.Url.Authority;
            //}

            HostName = "http://80.234.43.78";
            base.Initialize(requestContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            filterContext.Result = Redirect(ErrorPage);
        }
    }
}