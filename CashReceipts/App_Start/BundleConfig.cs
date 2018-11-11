using System.Web;
using System.Web.Optimization;

namespace CashReceipts
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

            bundles.Add(new ScriptBundle("~/bundles/receipt-headers").Include(
                "~/Scripts/CashReceipts/ReceiptHeaders.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Scripts/KenodUI2017.1.223/styles/Kendo").Include(
                "~/Scripts/KenodUI2017.1.223/styles/kendo.common.min.css",
                "~/Scripts/KenodUI2017.1.223/styles/kendo.bootstrap.min.css",
                "~/Scripts/sweetalert/sweetalert.css",
                "~/Content/KendoCustomization.css"));

            bundles.Add(new StyleBundle("~/Content/allcss").Include(
                "~/Content/bootstrap3.3.5.min.css",
                "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/jquery-2.1.4.min.js",
                "~/Scripts/bootstrap3.3.5.min.js",
                "~/Scripts/KenodUI2017.1.223/js/kendo.all.min.js",
                "~/Scripts/KenodUI2017.1.223/js/jszip.min.js",
                "~/Scripts/sweetalert/sweetalert.min.js",
                "~/Scripts/helpers.js",
                "~/Scripts/SwetAlertHelper.js"));
        }
    }
}
