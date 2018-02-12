using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PCSReports.Models;

namespace PCSReports.Controllers
{
    public class ReportModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReportModels
        public ActionResult Index()
        {
            return View(db.ReportModels.Where(x=>x.isActive==true).OrderBy(x=>x.name).ToList());
        }

        public ActionResult IndexAll()
        {
            return View(db.ReportModels.OrderBy(x => x.name).ToList());
        }
        // GET: ReportModels/Details/5
        public ActionResult Details(int? id)
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

        // GET: ReportModels/Create
        public ActionResult Create()
        {
            ReportViewModel vm = new ReportViewModel();
            ReportService2005.ReportingService2005 rs = new ReportService2005.ReportingService2005();
            ReportService2005.CatalogItem[] items = rs.ListChildren("/", true);
            List<SelectListItem> slItems = new List<SelectListItem>();
            foreach(ReportService2005.CatalogItem item in items)
            {
                if(item.Type==ReportService2005.ItemTypeEnum.Report || item.Type==ReportService2005.ItemTypeEnum.LinkedReport)
                {
                    SelectListItem sli = new SelectListItem();
                    sli.Value = item.Path;
                    sli.Text = item.Name + " || " + item.Path;
                    slItems.Add(sli);
                }
            }
            SelectList sl = new SelectList(slItems);
            vm.lsReports = sl;
            return View(vm);
        }

        // POST: ReportModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,path")] ReportModel reportModel)
        {
            if (ModelState.IsValid)
            {
                if (db.ReportModels.Where(x => x.name == reportModel.name && x.path == reportModel.path).Any())
                {
                    ViewBag.ErrorMessage = "A report already exists with this Name and Path.";
                    return View(reportModel);
                }
                else
                {
                    reportModel.ts = DateTime.Now;
                    reportModel.isActive = true;
                    db.ReportModels.Add(reportModel);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(reportModel);
        }

        // GET: ReportModels/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: ReportModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,path,isActive")] ReportModel reportModel)
        {
            if (ModelState.IsValid)
            {
                reportModel.ts = DateTime.Now;
                db.Entry(reportModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reportModel);
        }

        // GET: ReportModels/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            ReportModel reportModel = db.ReportModels.Find(id);
            //db.ReportModels.Remove(reportModel);
            reportModel.ts = DateTime.Now;
            reportModel.isActive = false;
            db.Entry(reportModel).State = EntityState.Modified;
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
