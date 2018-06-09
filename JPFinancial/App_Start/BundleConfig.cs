using System.Web.Optimization;

namespace JPFinancial
{

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/SalaryScript.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-2.8.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/respond.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/site.css"));
        }
    }
    //public class BundleConfig
    //{
    //    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    //    public static void RegisterBundles(BundleCollection bundles)
    //    {
    //        bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
    //                    "~/Scripts/jquery-3.3.1.min.js"));

    //        bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
    //                    "~/Scripts/jquery.validate.min.js"));

    //        // Use the development version of Modernizr to develop with and learn from. Then, when you're
    //        // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
    //        bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
    //                    "~/Scripts/modernizr-2.8.3.js"));

    //        bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
    //                  "~/Scripts/bootstrap.min.js",
    //                  "~/Scripts/respond.min.js"));

    //        bundles.Add(new StyleBundle("~/Content/css").Include(
    //                  "~/Content/bootstrap.min.css",
    //                  "~/Content/site.css"));


    //        //Black-Forest Bundles
    //        bundles.Add(new ScriptBundle("~/bundles/black-forest/jquery").Include(
    //                  "~/black-forest/js/alertify.min.js",
    //                  "~/black-forest/js/fullcalendar.js",
    //                  "~/black-forest/js/jquery-ui-1.8.23.custom.min.js",
    //                  "~/black-forest/js/jquery.bootstrap.wizard.js",
    //                  "~/black-forest/js/jquery.dataTables.js",
    //                  "~/black-forest/js/jquery.easy-pie-chart.js",
    //                  "~/black-forest/js/jquery.min.js",
    //                  "~/black-forest/js/jquery.scrollUp.js",
    //                  "~/black-forest/js/jquery.sparkline.js",
    //                  "~/black-forest/js/load-image.min.js",
    //                  "~/black-forest/js/moment.js",
    //                  "~/black-forest/js/tiny-scrollbar.js"));

    //        bundles.Add(new ScriptBundle("~/bundles/black-forest/bootstrap").Include(
    //                  "~/black-forest/js/bootstrap-colorpicker.js",
    //                  "~/black-forest/js/bootstrap-editable.min.js",
    //                  "~/black-forest/js/bootstrap-image-gallery-main.js",
    //                  "~/black-forest/js/bootstrap-image-gallery.js",
    //                  "~/black-forest/js/bootstrap-timepicker.js",
    //                  "~/black-forest/js/bootstrap.js"));

    //        bundles.Add(new StyleBundle("~/Content/black-forest/css").Include(
    //                  "~/black-forest/css/alertify.core.css",
    //                  "~/black-forest/css/bootstrap-editable.css",
    //                  "~/black-forest/css/calendar-theme.css",
    //                  "~/black-forest/css/charts-graphs.css",
    //                  "~/black-forest/css/fullcalendar.css",
    //                  "~/black-forest/css/main.css",
    //                  "~/black-forest/css/select2.css",
    //                  "~/black-forest/css/style.css",
    //                  "~/black-forest/css/timepicker.css"));
    //    }
    //}
}
