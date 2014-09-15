using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestLibrary.Models
{
    public enum Status
    {
        Available,Borrowed,Reserved,Lost
    }
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

        [DefaultValue(Status.Available)]
        [Column("Status")]
        public Status BookStatus { get; set; }
        public virtual ICollection<BorrowEntry> BorrowEntries { get; set; }
        public virtual ICollection<RequestEntry> RequestEntries { get; set; }
    }
}