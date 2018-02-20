using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PCSReports.Models;

namespace PCSReports.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReportModels
        public ActionResult Index()
        {
            return View(db.ReportModels.Where(x=>x.isActive==true).OrderBy(x=>x.name).ToList());
        }


        // GET: ReportModels/Create
        [Audit]
        public ActionResult Create()
        {
            ReportViewModel vm = new ReportViewModel();
            SelectList sl = Utility.GetReportsDDL();
            vm.lsReports = sl;
            return View(vm);
        }

        // POST: ReportModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult Create(ReportViewModel reportviewModel)
        {
            ReportModel reportModel = reportviewModel.rm;
            if (db.ReportModels.Where(x => x.path == reportModel.path).Any())
            {
                ViewBag.ErrorMessage = "This Report already exists.";
                SelectList sl = Utility.GetReportsDDL();
                reportviewModel.lsReports = sl;
                return View(reportviewModel);
            }
            else
            {
                ReportService2005.ReportingService2005 rs = new ReportService2005.ReportingService2005();
                rs.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["userName"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                rs.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SOAPTimeout"].ToString());
                rs.Url = ConfigurationManager.AppSettings["webServiceURL"].ToString();
                ReportService2005.CatalogItem[] items = rs.ListChildren("/", true);
                ReportService2005.CatalogItem item = items.Where(x => x.Path == reportModel.path).Single();
                reportModel.name = item.Name;
                reportModel.description = item.Description;
                reportModel.ts = DateTime.Now;
                reportModel.isActive = true;
                db.ReportModels.Add(reportModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        // GET: ReportModels/Delete/5
        [Audit]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportModel reportModel = db.ReportModels.Find(id);
            if (reportModel == null)
            {
                return HttpNotFound();
            }
            return View(reportModel);
        }

        // POST: ReportModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportModel reportModel = db.ReportModels.Find(id);
            db.ReportModels.Remove(reportModel);
            db.SaveChanges();
            return RedirectToAction("Index");
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
