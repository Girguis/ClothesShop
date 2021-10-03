using System.Web.Optimization;

namespace ClothesShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.twbsPagination-1.3.1.js", "~/Scripts/General/TableManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootbox.min.js"
                      , "~/Scripts/General/CommonManager.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css", "~/Content/style.css"));

            bundles.Add(new StyleBundle("~/Content/fontAwesome")
                .Include("~/Content/font-awesome.css"
                ));
            bundles.Add(new StyleBundle("~/Content/Login")
               .Include("~/Content/Account/login.css"
               ));

            /* Layout */
            bundles.Add(new ScriptBundle("~/Scripts/Layout")
                .Include("~/Scripts/Layout/LayoutManager.js")
                 .Include("~/Scripts/Common/General.js")
                );
            bundles.Add(new StyleBundle("~/Content/Layout")
              .Include("~/Content/Layout/Layout.css")
              .Include("~/Content/General/CommonStyle.css")
              );
            bundles.Add(new StyleBundle("~/Content/Layout-ar")
              .Include("~/Content/Layout/Layout-ar.css")
              );


            /* Moment.js */
            bundles.Add(new ScriptBundle("~/Scripts/moment")
                .Include("~/Scripts/moment.min.js"));

            bundles.Add(new StyleBundle("~/Styles/EmployeeAdd")
               .Include("~/Content/Employee/Add.css"));
            bundles.Add(new ScriptBundle("~/Scripts/Employee")
               .Include("~/Scripts/Employees/EmployeesManager.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TodayExpenses")
               .Include("~/Scripts/TodayExpenses/TodayExpensesManager.js"));

            bundles.Add(new ScriptBundle("~/Scripts/TodaySales")
                   .Include("~/Scripts/TodaySales/TodaySalesManager.js"));

            bundles.Add(new ScriptBundle("~/Scripts/select")
                   .Include("~/Scripts/bootstrap-select.min.js"));

            bundles.Add(new StyleBundle("~/Styles/select")
                   .Include("~/Content/bootstrap-select.min.css"));

            bundles.Add(new StyleBundle("~/Styles/select-ar")
                   .Include("~/Content/General/select-ar.css"));

            bundles.Add(new ScriptBundle("~/Scripts/Orders")
              .Include("~/Scripts/Orders/OrdersManager.js"));

            bundles.Add(new ScriptBundle("~/Scripts/chart").Include(
                       "~/Scripts/Chart.min.js", "~/Scripts/Statistics/StatisticsManager.js"));

            bundles.Add(new StyleBundle("~/Styles/chart").Include(
                       "~/Content/Chart.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/farbtastic").Include(
                       "~/Scripts/farbtastic.js"));

            bundles.Add(new StyleBundle("~/Styles/farbtastic").Include(
                       "~/Content/farbtastic.css"));

            bundles.Add(new ScriptBundle("~/Scripts/print").Include(
                     "~/Scripts/Print/PrintManager.js"));

        }
    }
}
