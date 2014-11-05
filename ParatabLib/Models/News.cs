using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace ParatabLib.Models
{
    //This class is representation of News.
    public class News
    {
        private int _NewsID;
        public int NewsID { get { return _NewsID; } set { _NewsID = value; } }

        private string _Title;
        [Required(ErrorMessage="News title is required.")]
        public string Title { get { return _Title; } set { _Title = value; } }

        private string _Detail;
        [Required(ErrorMessage = "News detail is required.")]
        [AllowHtml]
        public string Detail { get { return _Detail; } set { _Detail = value; } }

        private DateTime _PostTime;
        public DateTime PostTime { get { return _PostTime; } set { _PostTime = value; } }
    }
}