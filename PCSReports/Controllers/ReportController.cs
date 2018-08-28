using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Configuration;

namespace PCSReports.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Index(string currentFilter, string SearchFilter, int? page)
        {
            if (SearchFilter != null)
            {
                page = 1;
            }
            else
            {
                SearchFilter = currentFilter;
            }
            ViewBag.CurrentFilter = SearchFilter;
            string Username = User.Identity.Name;
            List<ReportModel> reports = Utility.GetReportListForUser(Username);
            if (!String.IsNullOrEmpty(SearchFilter))
            {
                reports = reports.Where(s => s.name.ToLower().Contains(SearchFilter.ToLower())).ToList();
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            ReportUserViewModel vm = new ReportUserViewModel();
            vm.lsReportsForUser = reports.ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        // GET: Report/ViewReport/5
        [Audit]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ViewReport(int? id, int Width, int Height)
        {
            ReportViewModel vm = new ReportViewModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportModel reportModel = db.ReportModels.Find(id);
            if (reportModel == null)
            {
                return HttpNotFound();
            }
            string Username = User.Identity.Name;
            List<ReportModel> lsrm = Utility.GetReportListForUser(Username);
            if(!lsrm.Where(x=> x.Id==id).Any())
            {
                return RedirectToAction("Index");
            }
            vm.rm = reportModel;
            vm.Height = Height;
            vm.Width = Width;
            string sUrlBase = ConfigurationManager.AppSettings["UrlBase"];
            string sUrlpath = String.Format(sUrlBase + "ViewReport.aspx?Path={0}&Height={1}", vm.rm.path, Height);
            vm.ReportURL = sUrlpath;
            vm.lsOuputs = Utility.GetOutputs();
            return View(vm);
        }

    }
}
