﻿using System;
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
    public class PortalMenusModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult _PortalMenu()
        {
            return PartialView(db.PortalMenusModels.ToList());
        }
        // GET: PortalMenusModels
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Index()
        {
            PortalMenuViewModel vm = new PortalMenuViewModel();
            vm.lsMenus = db.PortalMenusModels.ToList();
            IList<SelectListItem> lsItems = Utility.GetRoles();
            SelectListItem sliAll = new SelectListItem { Value = "All", Text = "All" };
            lsItems.Insert(0, sliAll);
            vm.slRoles = new SelectList(lsItems, "Text", "Text");
            vm.slParentMenus = Utility.GetParentMenus();
            vm.slMenuTypes = Utility.GetMenuTypes();
            return View(vm);
        }

        // GET: PortalMenusModels/Create
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Create()
        {
            PortalMenuViewModel vm = new PortalMenuViewModel();
            vm.menusModel = new PortalMenusModel();
            IList<SelectListItem> lsItems = Utility.GetRoles();
            SelectListItem sliAll = new SelectListItem { Value = "All", Text = "All" };
            lsItems.Insert(0, sliAll);
            vm.slRoles = new SelectList(lsItems, "Text", "Text");
            vm.slParentMenus = Utility.GetParentMenus();
            vm.slMenuTypes = Utility.GetMenuTypes();
            return View(vm);
        }

        // POST: PortalMenusModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Create([Bind(Include = "ID,MenuName,ControllerName,ActionName,MenuType,ParentID,Sortorder,RoleName")] PortalMenusModel portalMenusModel)
        {
            if (ModelState.IsValid)
            {
                db.PortalMenusModels.Add(portalMenusModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(portalMenusModel);
        }

        // GET: PortalMenusModels/Edit/5
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PortalMenusModel portalMenusModel = db.PortalMenusModels.Find(id);
            if (portalMenusModel == null)
            {
                return HttpNotFound();
            }
            PortalMenuViewModel vm = new PortalMenuViewModel();
            vm.menusModel = portalMenusModel;
            IList<SelectListItem> lsItems = Utility.GetRoles();
            SelectListItem sliAll = new SelectListItem { Value = "All", Text = "All" };
            lsItems.Insert(0, sliAll);
            vm.slRoles = new SelectList(lsItems, "Text", "Text");
            vm.slParentMenus = Utility.GetParentMenus();
            vm.slMenuTypes = Utility.GetMenuTypes();
            return View(vm);
        }

        // POST: PortalMenusModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Edit([Bind(Include = "ID,MenuName,ControllerName,ActionName,MenuType,ParentID,Sortorder,RoleName")] PortalMenusModel portalMenusModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(portalMenusModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PortalMenuViewModel vm = new PortalMenuViewModel();
            vm.menusModel = portalMenusModel;
            IList<SelectListItem> lsItems = Utility.GetRoles();
            SelectListItem sliAll = new SelectListItem { Value = "All", Text = "All" };
            lsItems.Insert(0, sliAll);
            vm.slRoles = new SelectList(lsItems, "Text", "Text");
            vm.slParentMenus = Utility.GetParentMenus();
            vm.slMenuTypes = Utility.GetMenuTypes();
            return View(vm);
        }

        // GET: PortalMenusModels/Delete/5
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PortalMenusModel portalMenusModel = db.PortalMenusModels.Find(id);
            if (portalMenusModel == null)
            {
                return HttpNotFound();
            }
            return View(portalMenusModel);
        }

        // POST: PortalMenusModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Audit]
        public ActionResult DeleteConfirmed(int id)
        {
            PortalMenusModel portalMenusModel = db.PortalMenusModels.Find(id);
            db.PortalMenusModels.Remove(portalMenusModel);
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
