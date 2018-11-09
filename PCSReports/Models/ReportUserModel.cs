using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCSReports.Models
{
    public class ReportUserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string username { get; set; }
        [Required]
        [Display(Name = "Report")]
        public int ReportID { get; set; }
        [Display(Name = "Is Active?")]
        public bool isActive { get; set; }
        [Display(Name = "Last Update")]
        public DateTime ts { get; set; }
        
        public int Views { get; set; }

        public ReportUserModel()
        {
            Views = 0;
        }
    }
}