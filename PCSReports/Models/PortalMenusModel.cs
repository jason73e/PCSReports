using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCSReports.Models
{
    public class PortalMenusModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string MenuName { get; set; }
        [Required]
        [Display(Name = "Controller")]
        public string ControllerName { get; set; }

        [Required]
        [Display(Name = "Action")]
        public string ActionName { get; set; }

        [Required]
        [Display(Name = "Menu Type")]
        public string MenuType { get; set; }

        [Required]
        [Display(Name ="Parent Menu")]
        public int ParentID { get; set; }

        [Required]
        [Display(Name = "Order")]
        public int Sortorder { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}