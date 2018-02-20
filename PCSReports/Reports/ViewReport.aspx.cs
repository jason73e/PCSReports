using Microsoft.Reporting.WebForms;
using PCSReports.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCSReports.Reports
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    string sUserName = ConfigurationManager.AppSettings["userName"].ToString();
                    string sPassword = ConfigurationManager.AppSettings["Password"].ToString();
                    string sUrl = ConfigurationManager.AppSettings["reportURL"].ToString();
                    string sPath = Request["Path"];

                    rvSiteMapping.Height = Unit.Pixel(Convert.ToInt32(Request["Height"]) - 58);
                    rvSiteMapping.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                    rvSiteMapping.ServerReport.ReportServerUrl = new Uri(sUrl);
                    IReportServerCredentials irsc = new CustomReportCredentials(sUserName, sPassword, "");
                    rvSiteMapping.ServerReport.ReportServerCredentials = irsc;
                    rvSiteMapping.ServerReport.ReportPath = sPath;

                    rvSiteMapping.ServerReport.Refresh();
                }
                catch (Exception ex)
                {

                }
            }

        }
    }
}