using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public class AuditLog
    {
        [Key]
        public Guid AuditID { get; set; }
        public string IPAddress { get; set; }
        public string UserName { get; set; }
        public string URLAccessed { get; set; }
        public DateTime TimeAccessed { get; set; }

        public AuditLog()
        {
        }
    }
    public class AuditAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;
            if (ConfigurationManager.AppSettings["Audit"] == "true")
            {
                string sPostData = request.Form.ToString();
                if(sPostData.Contains("__RequestVerificationToken"))
                {
                    int iStart = sPostData.IndexOf("__RequestVerificationToken");
                    int iEnd = sPostData.IndexOf("&", iStart)+1;
                    sPostData = sPostData.Remove(iStart, iEnd - iStart);

                }
                if(sPostData.Length>0)
                {
                    sPostData = " Post Data: " + sPostData;
                }

                //Generate an audit
                AuditLog audit = new AuditLog()
                {
                    AuditID = Guid.NewGuid(),
                    IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                    URLAccessed = request.RawUrl + sPostData,
                    TimeAccessed = DateTime.UtcNow,
                    UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                };

                //Stores the Audit in the Database
                ApplicationDbContext db = new ApplicationDbContext();
                db.AuditRecords.Add(audit);
                db.SaveChanges();

                base.OnActionExecuting(filterContext);
            }
        }
    }
}