using PCSReports.ReportService2005;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public class ReportViewModel
    {
        public ReportModel rm { get; set; }

        public string ReportURL { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ReportParameter[] lsrp { get; set; }
        public ReportParameterExtraData[] lsrped { get; set; }

        public SelectList lsOuputs { get; set; }

        public string sOutputType { get; set; }

        public SelectList lsReports { get; set; }
    }

    public class ReportParameterExtraData
    {
        public List<SelectListItem> lsVVSL { get; set; }
    }
}