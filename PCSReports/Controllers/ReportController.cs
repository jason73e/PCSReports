using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Configuration;
using Microsoft.Reporting.WebForms;

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
            Utility.ResetViews();
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
            List<ReportModel> commonreports = Utility.GetCommonReportListForUser(Username);
            List<ReportModel> reports = Utility.GetReportListForUser(Username);
            if (!String.IsNullOrEmpty(SearchFilter))
            {
                commonreports = commonreports.Where(s => s.name.ToLower().Contains(SearchFilter.ToLower())).ToList();
                reports = reports.Where(s => s.name.ToLower().Contains(SearchFilter.ToLower())).ToList();
            }
            foreach (ReportModel rm in commonreports)
            {
                reports.Insert(0, rm);
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
            if (!Utility.UserHasAccess(Username, id))
            {
                return RedirectToAction("Index");
            }
            Utility.UserViewReport(Username, id);
            vm.rm = reportModel;
            vm.Height = Height;
            vm.Width = Width;
            string sUrlBase = ConfigurationManager.AppSettings["UrlBase"];
            string sUrlpath = String.Format(sUrlBase + "ViewReport.aspx?Path={0}&Height={1}", vm.rm.path, Height);
            vm.ReportURL = sUrlpath;
            vm.lsOuputs = Utility.GetOutputs();
            return View(vm);
        }

        // GET: Report/ViewCustomReport/5
        [Audit]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ViewCustomReport(int? id, int Width, int Height)
        {
            string sUserName = ConfigurationManager.AppSettings["userName"].ToString();
            string sPassword = ConfigurationManager.AppSettings["Password"].ToString();
            string sUrl = ConfigurationManager.AppSettings["reportURL"].ToString();

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
            if(!Utility.UserHasAccess(Username,id))
            {
                return RedirectToAction("Index");
            }
            Utility.UserViewReport(Username, id);
            vm.rm = reportModel;
            ReportViewer reportviewer = new ReportViewer();
            reportviewer.ProcessingMode = ProcessingMode.Remote;
            reportviewer.ServerReport.ReportServerUrl = new Uri(sUrl);
            IReportServerCredentials irsc = new CustomReportCredentials(sUserName, sPassword, "");
            reportviewer.ServerReport.ReportServerCredentials = irsc;
            reportviewer.ServerReport.ReportPath = vm.rm.path;
            ReportParameterInfoCollection reportParameterInfos = reportviewer.ServerReport.GetParameters();
            vm.reportParameters = new List<ReportParameter>();
            foreach (ReportParameterInfo rpi in reportParameterInfos)
            {
                if (rpi.PromptUser && rpi.Visible)
                {
                    ReportParameter p = new ReportParameter();
                    p.Name = rpi.Name;
                    p.Visible = rpi.Visible;
                    vm.reportParameters.Add(p);
                }
            }
            vm.reportParameterInfoCollection = reportParameterInfos;
            vm.Height = Height;
            vm.Width = Width;
            string sUrlBase = ConfigurationManager.AppSettings["UrlBase"];
            string sUrlpath = String.Format(sUrlBase + "ViewCustomReport.aspx?Path={0}&Height={1}", vm.rm.path, Height);
            vm.ReportURL = sUrlpath;
            vm.lsOuputs = Utility.GetOutputs();
            return View(vm);
        }
    }
}
