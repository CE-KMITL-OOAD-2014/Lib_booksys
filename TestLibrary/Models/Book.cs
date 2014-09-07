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

        [DisplayFormat(NullDisplayText="Unknown author.")]
        public string Author { get; set; }

        [DisplayFormat(NullDisplayText="Unknown detail.")]
        public string Detail { get; set; }

        [DisplayFormat(NullDisplayText="Unknown year.")]
        public int? Year { get; set; }

        [DisplayFormat(NullDisplayText="Unknown publisher.")]
        public string Publisher { get; set; }

    }
}