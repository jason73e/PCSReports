using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<ReportModel> GetReportListForUser(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ReportModel> reports = db.ReportModels.Where(x => x.isActive == true).ToList();
            List<ReportModel> UserReports = new List<ReportModel>();
            foreach (ReportModel i in reports)
            {
                if (db.ReportUserModels.Where(x => x.username == Username && x.ReportID == i.Id && x.isActive == true).Any())
                {
                    UserReports.Add(i);
                }
            }
            return UserReports;
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
                output.Append(item.Text);
                output.Append("</td>");
                output.Append("</tr>");
            }

            output.Append("</table></div></div>");

            return new MvcHtmlString(output.ToString());
        }
    }
}