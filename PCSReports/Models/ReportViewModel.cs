using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public class ReportViewModel
    {
        public ReportModel rm { get; set; }

        public string ReportURL { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public SelectList lsOuputs { get; set; }

        public string sOutputType { get; set; }

        public SelectList lsReports { get; set; }

        public ReportParameterInfoCollection reportParameterInfoCollection { get; set; }

        public IList<ReportParameter> reportParameters { get; set; }
    }

}