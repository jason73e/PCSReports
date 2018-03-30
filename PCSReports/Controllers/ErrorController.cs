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
            AuditLog audit = new AuditLog()
            {
                AuditID = Guid.NewGuid(),
                IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress,
                TimeAccessed = DateTime.UtcNow,
                URLAccessed = Request.QueryString["aspxerrorpath"].ToString(),
                UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous",
                Message = "Error"
            };
            Utility.AddLog(audit);
            return View("Error");
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 200;
            AuditLog audit = new AuditLog()
            {
                AuditID = Guid.NewGuid(),
                IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress,
                TimeAccessed = DateTime.UtcNow,
                URLAccessed = Request.QueryString["aspxerrorpath"].ToString(),
                UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous",
                Message = "Not Found Error"
            };
            Utility.AddLog(audit);
            return View("NotFound");
        }

        public ActionResult InternalServer()
        {
            Response.StatusCode = 200;
            AuditLog audit = new AuditLog()
            {
                AuditID = Guid.NewGuid(),
                IPAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.UserHostAddress,
                TimeAccessed = DateTime.UtcNow,
                URLAccessed = Request.QueryString["aspxerrorpath"].ToString(),
                UserName = (Request.IsAuthenticated) ? HttpContext.User.Identity.Name : "Anonymous",
                Message = "Internal Server Error"
            };
            Utility.AddLog(audit);
            return View("InternalServer");
        }
    }
}