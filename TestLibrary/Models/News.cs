using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace TestLibrary.Models
{
    public class News
    {

        public int NewsID { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Detail { get; set; }

        
        public DateTime PostTime{get; set;}
    }
}