using System.Web.Optimization;

namespace ClothesShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
           // BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js", "~/Scripts/jquery.twbsPagination-1.3.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

     
            bundles.Add(new Bundle("~/bundles/common").Include(
                      "~/Js/Const.js",
                      "~/Js/CommonManager.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/dx.all.js"));

            bundles.Add(new ScriptBundle("~/bundles/orders")
                .Include("~/Js/Orders/OrdersManager.js")); 

            bundles.Add(new ScriptBundle("~/bundles/accounts")
                .Include("~/Js/Accounts/AccountsManager.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/shipmentCompanies")
                .Include("~/Js/ShipmentCompanies/ShipmentCompaniesManager.js"));   

            bundles.Add(new ScriptBundle("~/bundles/sizes")
                .Include("~/Js/Sizes/SizesManager.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/products")
                .Include("~/Js/Products/ProductsManager.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/employees")
                .Include("~/Js/Employees/EmployeesManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/employeeStatistics")
                .Include("~/Js/EmployeeStatistics/EmployeeStatisticsManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/singleSellerStatictics")
                        .Include("~/Js/EmployeeStatistics/SingleSellerStatisticsManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/employeesBalance")
                .Include("~/Js/EmployeeBalance/EmployeeBalanceManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/employeeBalanceInfo")
                .Include("~/Js/EmployeeBalance/EmployeeBalanceInfoManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/employeesInfo")
                .Include("~/Js/Employees/EmployeesInfoManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/deliveries")
                .Include("~/Js/Deliveries/DeliveriesManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/deliveriesInfo")
                .Include("~/Js/Deliveries/DeliveriesInfoManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/todaySales")
                .Include("~/Js/TodaySales/TodaySalesManager.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/todaySalesInfo")
                .Include("~/Js/TodaySales/TodaySalesInfoManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/todayExpens")
                .Include("~/Js/TodayExpens/TodayExpensManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/todayTransactions")
                .Include("~/Js/TodayTransactions/TodayTransactionsManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/todayExpensInfo")
                .Include("~/Js/TodayExpens/TodayExpensInfoManager.js"));   

            bundles.Add(new ScriptBundle("~/bundles/SystemData")
                .Include("~/Js/SystemData/SystemDataManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/suppliers")
                .Include("~/Js/Suppliers/SuppliersManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/statistics")
                .Include("~/Js/Statistics/StatisticsManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/Monthlystatistics")
                .Include("~/Js/Statistics/MonthlyStatisticsManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/productSuppliers")
                .Include("~/Js/ProductSuppliers/ProductSuppliersManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/productsSummary")
                .Include("~/Js/ProductsSummary/ProductsSummaryManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/Colors")
                .Include("~/Js/Colors/ColorsManager.js"));
  
            bundles.Add(new ScriptBundle("~/bundles/orderInfo")
                .Include("~/Js/Orders/OrderInfoManager.js")); 
            
            bundles.Add(new ScriptBundle("~/bundles/salesRates")
                .Include("~/Js/SalesRates/SalesRatesManager.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/productSupplierInfo")
                .Include("~/Js/ProductSuppliers/ProductSupplierInfoManager.js"));

            bundles.Add(new StyleBundle("~/Content/style").Include(
                      "~/Content/dx.light.css",
                      "~/Content/css/font.css" 
                      ));
            
            bundles.Add(new StyleBundle("~/Content/common")
              .Include("~/Content/General/CommonStyle.css")
              );
      
        }
    }
}
