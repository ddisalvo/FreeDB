namespace FreeDB.Web.App_Start
{
    using System;
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(
              new ScriptBundle("~/scripts/vendor")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/knockout-{version}.debug.js")
                .Include("~/scripts/knockout.mapping-latest.debug.js")
                .Include("~/scripts/sammy-{version}.js")
                .Include("~/scripts/toastr.js")
                .Include("~/scripts/Q.js")
                .Include("~/scripts/breeze.debug.js")
                .Include("~/scripts/bootstrap.js")
                .Include("~/scripts/moment.js")
              );

            bundles.Add(
              new StyleBundle("~/Content/css")
                .Include("~/Content/ie10mobile.css")
                .Include("~/Content/bootstrap/bootstrap.css")
                .Include("~/Content/bootstrap/bootstrap-responsive.css")
                //.Include("~/Content/durandal.css")
                .Include("~/Content/font-awesome.css")
                .Include("~/Content/font-awesome-ie7.min.css")
                .Include("~/Content/toastr.css")
                .Include("~/Content/app.css")
              );
        }

        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
            {
                throw new ArgumentNullException("ignoreList");
            }

            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");

            //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }
    }
}