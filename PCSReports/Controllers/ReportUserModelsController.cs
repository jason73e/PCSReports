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
    [Authorize(Roles = "Admin")]
    public class ReportUserModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReportUserModels
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
            CleanReportUsers();
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            ReportUserViewModel vm = new ReportUserViewModel();
            List<ReportUserModel> ls = db.ReportUserModels.Where(x => x.isActive == true).OrderBy(x => x.username).ToList();
            ls = ls.Where(x => x.username == SearchFilter).ToList();
            vm.lsReportUsers = ls.ToPagedList(pageNumber, pageSize);
            vm.lsReports = Utility.GetReports();
            vm.lsUser = Utility.GetUsers();
            return View(vm);
        }

        private void CleanReportUsers()
        {
            List<int> lsReportIds = db.ReportModels.Select(x=>x.Id).ToList();
            List<int> lsReportUserIds = db.ReportUserModels.Select(x=>x.ReportID).ToList();
            List<int> lsOldReports = lsReportUserIds.Where(x => !lsReportIds.Any(x2 => x2 == x)).ToList();
            foreach (int iReportId in lsOldReports)
            {
                if (db.ReportUserModels.Any(x => x.ReportID == iReportId))
                {
                    List<ReportUserModel> lsReportUsers = db.ReportUserModels.Where(x => x.ReportID == iReportId).ToList();
                    db.ReportUserModels.RemoveRange(lsReportUsers);
                    db.SaveChanges();
                }
            }
        }


        // GET: ReportUserModels/Create
        [Audit]
        public ActionResult Create()
        {
            ReportUserViewModel vm = new ReportUserViewModel();
            //vm.lsReportUsers = new List<ReportUserModel>();
            vm.lsReports = Utility.GetReports();
            vm.lsUser = Utility.GetUsers();
            return View(vm);
        }
        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult FillReport(string username)
        {
            ReportUserViewModel vm = new ReportUserViewModel();
            if (username.Length > 0)
            {
                MultiSelectList sl = Utility.GetReportCheckboxList(username);
                MvcHtmlString result = Utility.CheckBoxList("chklistitem", sl);
                vm.reportChks = result.ToHtmlString();
            }
            else
            {
                vm.reportChks = string.Empty;
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        // POST: ReportUserModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult Create(ReportUserViewModel reportUserModel)
        {
            string sp = Request.Form["chklistitem"];
            List<string> lsReportIDs = new List<string>();
            string sUserName = reportUserModel.reportUser.username;
            if(sp!=null)
            {
                lsReportIDs = sp.Split(',').ToList();
            }

            RemoveReportsForUser(sUserName, lsReportIDs);

            AddReportsForUser(sUserName, lsReportIDs);

            return RedirectToAction("Index");
        }

        // GET: ReportUserModels/Create
        [Audit]
        public ActionResult Copy()
        {
            ReportUserViewModel vm = new ReportUserViewModel();
            vm.lsUser = Utility.GetUsers();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult Copy(ReportUserViewModel reportUserModel)
        {
            List<string> lsRemoveReports = Utility.GetAllReportListForUser(reportUserModel.CopyToUser).Select(x => x.Id.ToString()).ToList();
            List<string> lsAddReports = Utility.GetAllReportListForUser(reportUserModel.CopyFromUser).Select(x => x.Id.ToString()).ToList();
            if (lsRemoveReports.Count > 0)
            {
                RemoveReportsForUser(reportUserModel.CopyToUser, lsRemoveReports);
            }
            if (lsAddReports.Count > 0)
            {
                AddReportsForUser(reportUserModel.CopyToUser, lsAddReports);
            }
            return RedirectToAction("Index");
        }

        private void AddReportsForUser(string sUserName, List<string> lsReportIDs)
        {
            List<ReportUserModel> ls = new List<ReportUserModel>();
            foreach(string s in lsReportIDs)
            {
                ReportUserModel rum = new ReportUserModel();
                rum.username = sUserName;
                rum.ReportID = Convert.ToInt32(s);
                rum.isActive = true;
                rum.ts = DateTime.Now;
                ls.Add(rum);
            }
            foreach(ReportUserModel r in ls)
            {
                if(!db.ReportUserModels.Where(x=>x.username==r.username && x.ReportID==r.ReportID).Any())
                {
                    db.ReportUserModels.Add(r);
                    db.SaveChanges();
                }
            }
        }

        private void RemoveReportsForUser(string sUserName, List<string> lsReportIDs)
        {
            List<ReportUserModel> ls =  db.ReportUserModels.Where(x => x.username == sUserName && !lsReportIDs.Any(x1 => x1 == x.ReportID.ToString())).ToList();
            db.ReportUserModels.RemoveRange(ls);
            db.SaveChanges();
        }

        // GET: ReportUserModels/Delete/5
        [Audit]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportUserModel reportUserModel = db.ReportUserModels.Find(id);
            if (reportUserModel == null)
            {
                return HttpNotFound();
            }
            ReportUserViewModel vm = new ReportUserViewModel();
            vm.reportUser = reportUserModel;
            vm.lsUser = Utility.GetUsers();
            vm.lsReports = Utility.GetReports();
            return View(vm);
        }

        // POST: ReportUserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Audit]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportUserModel reportUserModel = db.ReportUserModels.Find(id);
            db.ReportUserModels.Remove(reportUserModel);
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
