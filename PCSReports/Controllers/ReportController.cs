using PCSReports.Models;
using PCSReports.ReportService2005;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
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

        // GET: Report/ViewReport/5
        public ActionResult ViewReportOld(int? id)
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
            vm.rm = reportModel;
            vm.lsOuputs = Utility.GetOutputs();
            ParameterValue[] rp = null;
            DataSourceCredentials[] dsc = null;
            ReportingService2005 reportingService = new ReportingService2005();
            reportingService.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["userName"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            reportingService.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SOAPTimeout"].ToString());
            reportingService.Url = ConfigurationManager.AppSettings["webServiceURL"].ToString();
            vm.lsrp = reportingService.GetReportParameters(vm.rm.path, null, true, rp, dsc);
            vm.lsrped = new ReportParameterExtraData[vm.lsrp.Length];
            for (int x = 0; x < vm.lsrp.Length; x++)
            {
                if(vm.lsrp[x].ValidValues!=null)
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach(ValidValue v in vm.lsrp[x].ValidValues)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = v.Label;
                        item.Value = v.Value;
                        items.Add(item);
                    }
                    vm.lsrped[x] = new ReportParameterExtraData();
                    vm.lsrped[x].lsVVSL = items;
                }
            }
            return View(vm);
        }

        // POST: Report/Create
        [HttpPost]
        public ActionResult ViewReportOld(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
