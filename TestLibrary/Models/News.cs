using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace TestLibrary.Models
{
    public class News
    {
        private int _NewsID;
        public int NewsID { get { return _NewsID; } set { _NewsID = value; } }

        private string _Title;
        [Required]
        public string Title { get { return _Title; } set { _Title = value; } }

        private string _Detail;
        [Required]
        [AllowHtml]
        public string Detail { get { return _Detail; } set { _Detail = value; } }

        private DateTime _PostTime;
        public DateTime PostTime { get { return _PostTime; } set { _PostTime = value; } }
    }
}