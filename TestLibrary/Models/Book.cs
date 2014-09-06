using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TestLibrary.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        [Required]
        public string BookName { get; set; }

        public string Author { get; set; }

        public string Detail { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }

    }
}