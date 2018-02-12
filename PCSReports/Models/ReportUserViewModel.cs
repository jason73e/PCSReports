using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public class ReportUserViewModel
    {
        public IEnumerable<ReportUserModel> lsReportUsers { get; set; }

        public IEnumerable<ReportModel> lsReportsForUser { get; set; }

        public ReportUserModel reportUser { get; set; }

        public SelectList lsUser { get; set; }

        public SelectList lsReports { get; set; }

        public string reportChks { get; set; }
    }
}