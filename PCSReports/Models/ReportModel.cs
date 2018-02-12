using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCSReports.Models
{
    public class ReportModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Name")]
        public string name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }
        [Required]
        [Display(Name = "Path")]
        public string path { get; set; }
        [Display(Name = "Is Active?")]
        public bool isActive { get; set; }

        [Display(Name = "Last Update")]
        public DateTime ts { get; set; }

    }
}