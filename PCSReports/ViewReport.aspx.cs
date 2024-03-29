﻿using Microsoft.Reporting.WebForms;
using PCSReports.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PCSReports.Reports
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        [OutputCache(NoStore = true, Duration = 0)]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (!IsPostBack)
                {
                    try
                    {
                        string sUserName = ConfigurationManager.AppSettings["userName"].ToString();
                        string sPassword = ConfigurationManager.AppSettings["Password"].ToString();
                        string sUrl = ConfigurationManager.AppSettings["reportURL"].ToString();
                        string sPath = Request["Path"];
                        if (sPath == string.Empty)
                        {
                            throw new Exception();
                        }
                        rvSiteMapping.Height = Unit.Pixel(Convert.ToInt32(Request["Height"]) - 58);
                        rvSiteMapping.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                        rvSiteMapping.ServerReport.ReportServerUrl = new Uri(sUrl);
                        IReportServerCredentials irsc = new CustomReportCredentials(sUserName, sPassword, "");
                        rvSiteMapping.ServerReport.ReportServerCredentials = irsc;
                        rvSiteMapping.ServerReport.ReportPath = sPath;
                        rvSiteMapping.KeepSessionAlive = false;
                       
                        rvSiteMapping.ServerReport.Refresh();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                Response.Redirect("~/Account/Login");
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            DatePickers.Value = string.Join(",", (new List<string>(GetDateParameters()).ToArray()));
        }
        private IList<string> GetDateParameters()
        {
            List<string> lsrv = new List<string>();
            // I'm assuming report view control id as reportViewer
            foreach (ReportParameterInfo info in rvSiteMapping.ServerReport.GetParameters())
            {
                if (info.DataType == ParameterDataType.DateTime)
                {
                    lsrv.Add(string.Format("[{0}]", info.Prompt));
                }
            }
            return lsrv;
        }

    }
}