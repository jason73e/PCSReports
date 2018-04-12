using PCSReports.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace PCSReports
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AntiForgeryConfig.SuppressXFrameOptionsHeader = false;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            if (httpException == null)
            {
                return;
            }

            var statusCode = httpException.GetHttpCode();
            Response.Clear();
            Server.ClearError();
            string sController = "Error";
            string sException = exception.Message;
            string sPath = Request.Url.ToString();
            string sAction = "Index";
            Response.TrySkipIisCustomErrors = true;
            Context.Server.ClearError();
            Response.Redirect(String.Format("~/{0}/{1}/?message={2}&aspxerrorpath={3}", sController,sAction, sException,sPath));

        }
    }
}
