using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace ClothesShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ResourceManager resourceManager = new ResourceManager(typeof(Languages.Resources));
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["UserLanguage"];
            if (cookie != null && cookie.Value != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookie.Value);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookie.Value);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ar");
            }
        }
        void Application_Error(object sender, EventArgs e)
        {
            Exception TheError = Server.GetLastError();
            Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, TheError);
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            if (TheError is HttpException && ((HttpException)TheError).GetHttpCode() == 404)
            {
                Response.Redirect("~/Error/NotFound");
            }
            else
            {
                Response.Redirect("~/Error/InternalError");
            }
        }
    }
}
