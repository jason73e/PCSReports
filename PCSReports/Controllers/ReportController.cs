using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PCSReports.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        public ActionResult Index()
        {
            string Username = User.Identity.Name;
            ReportUserViewModel vm = new ReportUserViewModel();
            vm.lsReportsForUser = Utility.GetReportListForUser(Username);
            return View(vm);
        }

        // GET: Report/ViewReport/5
        [Audit]
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
            vm.ReportURL = String.Format("../../Reports/ViewReport.aspx?Path={0}&Height={1}", vm.rm.path, Height);
            vm.lsOuputs = Utility.GetOutputs();
            return View(vm);
        }

    }
}
