using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
namespace TestLibrary.Models
{
    public class BorrowEntry
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Borrower")]
        public int UserID { get; set; }
        [ForeignKey("BorrowBook")]
        public int BookID { get; set; }

        [Column(TypeName="date")]
        public DateTime BorrowDate { get; set; }

        [Column(TypeName="date")]
        public DateTime DueDate { get; set; }

        [Column(TypeName="date")]
        [DisplayFormat(NullDisplayText="Not return")]
        public DateTime? ReturnDate { get; set; }

        [DefaultValue(0)]
        public short RenewCount { get; set; }

        public virtual Book BorrowBook { get; set; }
        public virtual Member Borrower { get; set; }

    }
}