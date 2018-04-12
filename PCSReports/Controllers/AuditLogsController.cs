using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PCSReports.Models;
using PagedList;

namespace PCSReports.Controllers
{
    [Authorize]
    public class AuditLogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AuditLogs
        public ActionResult Index(string currentURLFilter, string URLFilter, string currentIPAddressFilter, string IPAddressFilter, string currentUserFilter, string UserFilter, int? page)
        {
            if (UserFilter != null)
            {
                page = 1;
            }
            else
            {
                UserFilter = currentUserFilter;
            }
            if (IPAddressFilter != null)
            {
                page = 1;
            }
            else
            {
                IPAddressFilter = currentIPAddressFilter;
            }
            if (URLFilter != null)
            {
                page = 1;
            }
            else
            {
                URLFilter = currentURLFilter;
            }

            ViewBag.CurrentUserFilter = UserFilter;
            ViewBag.CurrentIPAddressFilter = IPAddressFilter;
            ViewBag.CurrentURLFilter = URLFilter;

            List<AuditLog> AuditRecords = db.AuditRecords.OrderByDescending(x => x.TimeAccessed).ToList();
            if (!String.IsNullOrEmpty(UserFilter))
            {
                AuditRecords = AuditRecords.Where(s => s.UserName.ToLower().Contains(UserFilter.ToLower())).ToList();
            }
            if(!String.IsNullOrEmpty(IPAddressFilter))
            {
                AuditRecords = AuditRecords.Where(s => s.IPAddress.ToLower().Contains(IPAddressFilter.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(URLFilter))
            {
                AuditRecords = AuditRecords.Where(s => (s.URLAccessed ?? "").ToLower().Contains(URLFilter.ToLower())).ToList();
            }
            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(AuditRecords.ToPagedList(pageNumber,pageSize));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
