using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public static class Utility
    {
        public static SelectList GetOutputs()
        {
            List<SelectListItem> ls = new List<SelectListItem>();
            SelectListItem sli = new SelectListItem();
            sli.Text = "PDF";
            sli.Value = "PDF";
            ls.Add(sli);
            sli = new SelectListItem();
            sli.Text = "EXCEL";
            sli.Value = "EXCEL";
            ls.Add(sli);
            sli = new SelectListItem();
            sli.Text = "CSV";
            sli.Value = "CSV";
            ls.Add(sli);
            sli = new SelectListItem();
            sli.Text = "TIFF";
            sli.Value = "TIFF";
            ls.Add(sli);
            sli = new SelectListItem();
            sli.Text = "MHTML";
            sli.Value = "MHTML";
            ls.Add(sli);
            return new SelectList(ls, "Value", "Text");
        }

        public static SelectList GetReports()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var reports = db.ReportModels.Where(x => x.isActive == true).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name + " " + x.path }).OrderBy(x=>x.Text);
            return new SelectList(reports, "Value", "Text");
        }
        public static SelectList GetUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var users = db.Users.Select(x => new SelectListItem { Value = x.UserName, Text = x.UserName }).OrderBy(x=>x.Text);
            return new SelectList(users, "Value", "Text");
        }

        public static SelectList GetMenuTypes()
        {
            IList<SelectListItem> menutypes = new List<SelectListItem>();
            SelectListItem sliMenu = new SelectListItem { Value = "Menu", Text = "Menu" };
            SelectListItem sliMenuItem = new SelectListItem { Value = "MenuItem", Text = "MenuItem" };

            menutypes.Insert(0, sliMenu);
            menutypes.Insert(0, sliMenuItem);

            SelectList sl = new SelectList(menutypes, "Value", "Text");
            return sl;

        }

        public static SelectList GetParentMenus()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            IList<SelectListItem> menus = db.PortalMenusModels.Where(x=> x.MenuType.ToLower()=="menu").Select(x => new SelectListItem { Value = x.ID.ToString(), Text = x.MenuName }).OrderBy(x => x.Text).ToList();
            SelectListItem sliRoot = new SelectListItem { Value = "0", Text = "Root" };
            menus.Insert(0,sliRoot);
            SelectList sl = new SelectList(menus, "Value", "Text");
            return sl;

        }
        public static List<ReportModel> GetAllReportListForUser(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ReportModel> reports = db.ReportModels.Where(x => x.isActive == true).OrderBy(x => x.name).ToList();
            List<ReportModel> UserReports = new List<ReportModel>();
            List<ReportUserModel> lsReportsforUser = db.ReportUserModels.Where(x => x.username == Username && x.isActive == true).ToList();
            if (lsReportsforUser.Count > 0)
            {
                foreach (ReportModel i in reports)
                {
                    if (lsReportsforUser.Where(x => x.ReportID == i.Id).Any())
                    {
                        UserReports.Add(i);
                    }
                }
            }
            return UserReports;
        }

        public static List<ReportModel> GetReportListForUser(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ReportModel> reports = db.ReportModels.Where(x => x.isActive == true).OrderBy(x=>x.name).ToList();
            List<ReportModel> UserReports = new List<ReportModel>();
            List<ReportUserModel> lsReportsforUser = db.ReportUserModels.Where(x => x.username == Username && x.isActive == true).ToList();
            int iAverageViews = 0;
            if (lsReportsforUser.Count > 0)
            {
                iAverageViews = Convert.ToInt32(lsReportsforUser.Average(x => x.Views));
                foreach (ReportModel i in reports)
                {
                    if (lsReportsforUser.Where(x => x.ReportID == i.Id && x.Views <= iAverageViews).Any())
                    {
                        UserReports.Add(i);
                    }
                }
            }
            return UserReports;
        }

        public static List<ReportModel> GetCommonReportListForUser(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ReportModel> reports = db.ReportModels.Where(x => x.isActive == true).OrderBy(x => x.name).ToList();
            List<ReportModel> UserReports = new List<ReportModel>();
            List<ReportUserModel> lsReportsforUser = db.ReportUserModels.Where(x => x.username == Username && x.isActive == true).ToList();
            int iAverageViews = 0;
            if (lsReportsforUser.Count > 0)
            {
                iAverageViews = Convert.ToInt32(lsReportsforUser.Average(x => x.Views));
                foreach (ReportModel i in reports)
                {
                    if (lsReportsforUser.Where(x => x.ReportID == i.Id && x.Views > iAverageViews).Any())
                    {
                        UserReports.Add(i);
                    }
                }
            }
            return UserReports;
        }

        public static bool UserHasAccess(string username, int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return (db.ReportUserModels.Where(x => x.username == username && x.ReportID == id && x.isActive == true).Any());
        }

        public static void UserViewReport(string username, int? id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (db.ReportUserModels.Where(x => x.username == username && x.ReportID == id && x.isActive == true).Any())
            {
                ReportUserModel r = db.ReportUserModels.Where(x => x.username == username && x.ReportID == id && x.isActive == true).Single();
                r.Views = r.Views + 1;
                db.SaveChanges();
            }
                
            
        }

        public static IList<SelectListItem> GetRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            return db.Roles.Select(m => new SelectListItem { Value = m.Id, Text = m.Name }).ToList();
        }

        public static MultiSelectList GetReportCheckboxList(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var reports = db.ReportModels.Where(x => x.isActive == true).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.name + " " + x.path }).OrderBy(x=>x.Text).ToList();
            MultiSelectList result =  new MultiSelectList(reports, "Value", "Text");
            List<SelectListItem> slUpdates = new List<SelectListItem>();
            List<string> selectedItems = new List<string>();
            foreach(SelectListItem i in result)
            {
                if(db.ReportUserModels.Where(x=>x.username==Username && x.ReportID.ToString()==i.Value && x.isActive==true).Any())
                {
                    i.Selected = true;
                    slUpdates.Add(i);
                    selectedItems.Add(i.Value);
                }
                else
                {
                    slUpdates.Add(i);
                }
            }
            result = new MultiSelectList(slUpdates, "Value", "Text",selectedItems);
            return result;
        }
        public static MvcHtmlString CheckBoxList(string name, IEnumerable<SelectListItem> items)
        {
            var output = new StringBuilder();
            output.Append(@"<div class=""form-group""><div class=""checkbox""><table class=""table table-striped table-hover"">");

            foreach (var item in items)
            {
                output.Append("<tr>");
                output.Append("<td>");
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\" id=\"");
                output.Append(name + item.Value);
                output.Append("\" ");

                if (item.Selected)
                    output.Append(@" checked=""checked""");

                output.Append(" />");
                output.Append("</td>");
                output.Append("<td>");
                output.Append(item.Text + " | " + item.Value );
                output.Append("</td>");
                output.Append("</tr>");
            }

            output.Append("</table></div></div>");

            return new MvcHtmlString(output.ToString());
        }

        public static SelectList GetReportsDDL()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            ReportService2005.ReportingService2005 rs = new ReportService2005.ReportingService2005();
            rs.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["userName"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            rs.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SOAPTimeout"].ToString());
            rs.Url = ConfigurationManager.AppSettings["webServiceURL"].ToString();
            ReportService2005.CatalogItem[] items = rs.ListChildren("/", true);
            List<SelectListItem> slItems = new List<SelectListItem>();
            foreach (ReportService2005.CatalogItem item in items)
            {
                if (item.Type == ReportService2005.ItemTypeEnum.Report || item.Type == ReportService2005.ItemTypeEnum.LinkedReport)
                {
                    if (!db.ReportModels.Where(x => x.path == item.Path).Any())
                    {
                        SelectListItem sli = new SelectListItem();
                        sli.Value = item.Path;
                        sli.Text = item.Name + " || " + item.Path;
                        slItems.Add(sli);
                    }
                }
            }
            SelectList sl = new SelectList(slItems, "Value", "Text");
            return sl;
        }

        public static void AddLog(AuditLog log)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.AuditRecords.Add(log);
            db.SaveChanges();
        }

    }
}