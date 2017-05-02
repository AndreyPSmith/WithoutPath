using System.Web;
using System.Web.Optimization;

namespace WithoutPath
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                   "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                   "~/Scripts/date.format.js",
                   "~/Scripts/common.js",
                   "~/Scripts/array-equals.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular/angular.min.js",
                        "~/Scripts/angular/q.js",
                        "~/Scripts/angular/ui-bootstrap-tpls-2.5.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/textangular").Include(
                       "~/Scripts/angular/textAngular-rangy.min.js",
                       "~/Scripts/angular/textAngular-sanitize.js",
                       "~/Scripts/angular/textAngular.js",
                       "~/Scripts/angular/textAngularSetup.js"));

               bundles.Add(new ScriptBundle("~/bundles/withoutusers").Include(
                        "~/Scripts/Admin/users-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/withoutusers").Include(
                        "~/Scripts/Admin/users-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/withoutcorporations").Include(
                       "~/Scripts/Admin/corporations-controller.js"));

            bundles.Add(new ScriptBundle("~/bundles/withoutsystems").Include(
                  "~/Scripts/Admin/systems-controller.js"));


            bundles.Add(new ScriptBundle("~/bundles/connectthree").Include(
                  "~/Scripts/ConnectThree/EasePack.min.js",
                  "~/Scripts/ConnectThree/rAF.js",
                  "~/Scripts/ConnectThree/TweenLite.min.js",
                  "~/Scripts/ConnectThree/connect-three.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/withoutmap").Include(                
                   "~/Scripts/Map/konva.min.js",
                   "~/Scripts/Map/Vector.js",
                   "~/Scripts/Map/Link.js",
                   "~/Scripts/Map/SpaceSystem.js",
                   "~/Scripts/Map/Empire.js",
                   "~/Scripts/Map/Wormhole.js",
                   "~/Scripts/Map/without-path.js"));

            bundles.Add(new ScriptBundle("~/bundles/gridstack").Include(
                "~/Scripts/gridstack/lodash.min.js",
                 "~/Scripts/gridstack/gridstack.js",
                "~/Scripts/gridstack/gridstack.jQueryUI.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",                    
                      "~/Content/site.css"));
        }
    }
}
