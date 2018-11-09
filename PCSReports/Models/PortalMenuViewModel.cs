using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCSReports.Models
{
    public class PortalMenuViewModel
    {
        public PortalMenusModel menusModel { get; set; }

        public SelectList slRoles { get; set; }

        public SelectList slParentMenus { get; set; }

        public SelectList slMenuTypes { get; set; }

        public List<PortalMenusModel> lsMenus {get;set;}

    }
}