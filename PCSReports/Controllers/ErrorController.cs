using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 200;
            AuditLog audit = new AuditLog();
            String sPath = string.Empty;
            string sMessage = string.Empty;
            try
            {
                sMessage = "Error Exception: " + Request.QueryString["exception"].ToString();
            }
            catch(Exception e)
            {
                sMessage = "Error";
            }

            try
            {
                sMessage += " Error Message: " + Request.QueryString["message"].ToString();
            }
            catch (Exception e)
            {
                sMessage += " Error";
            }

            try
            {
                sPath = Request.QueryString["aspxerrorpath"].ToString();
            }
            catch(Exception e)
            {
                sPath = Request.Url.ToString();
            }

            audit.AuditID = Guid.NewGuid();
            audit.IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress;
            audit.TimeAccessed = DateTime.UtcNow;
            audit.URLAccessed = sPath;
            audit.UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous";
            audit.Message = sMessage;
            
            Utility.AddLog(audit);
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 200;
            AuditLog audit = new AuditLog();
            String sPath = string.Empty;
            string sMessage = string.Empty;
            try
            {
                sMessage = "Not Found Error " + Request.QueryString["exception"].ToString();
            }
            catch (Exception e)
            {
                sMessage = "Not Found Error";
            }

            try
            {
                sMessage += " Error Message: " + Request.QueryString["message"].ToString();
            }
            catch (Exception e)
            {
                sMessage += " Not Found Error";
            }


            try
            {
                sPath = Request.QueryString["aspxerrorpath"].ToString();
            }
            catch (Exception e)
            {
                sPath = Request.Url.ToString();
            }

            audit.AuditID = Guid.NewGuid();
            audit.IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress;
            audit.TimeAccessed = DateTime.UtcNow;
            audit.URLAccessed = sPath;
            audit.UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous";
            audit.Message = sMessage;

            Utility.AddLog(audit);
            return View("NotFound");
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 200;
            AuditLog audit = new AuditLog();
            String sPath = string.Empty;
            string sMessage = string.Empty;
            try
            {
                sMessage = "Internal Server Error " + Request.QueryString["exception"].ToString();
            }
            catch (Exception e)
            {
                sMessage = "Internal Server Error";
            }

            try
            {
                sMessage += " Error Message: " + Request.QueryString["message"].ToString();
            }
            catch (Exception e)
            {
                sMessage += " Internal Server Error";
            }
            try
            {
                sPath = Request.QueryString["aspxerrorpath"].ToString();
            }
            catch (Exception e)
            {
                sPath = Request.Url.ToString();
            }

            audit.AuditID = Guid.NewGuid();
            audit.IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress;
            audit.TimeAccessed = DateTime.UtcNow;
            audit.URLAccessed = sPath;
            audit.UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous";
            audit.Message = sMessage;

            Utility.AddLog(audit);
            return View("InternalServer");
        }
    }
}