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
        private int _ID;
        [Key]
        public int ID { get { return _ID; } set { _ID = value; } }

        private int _UserID;
        [ForeignKey("Borrower")]
        public int UserID { get { return _UserID; } set { _UserID = value; } }

        private int _BookID;
        [ForeignKey("BorrowBook")]
        public int BookID { get { return _BookID; } set { _BookID = value; } }

        private DateTime _BorrowDate;
        [Column(TypeName="date")]
        public DateTime BorrowDate { get { return _BorrowDate; } set { _BorrowDate = value; } }

        private DateTime _DueDate;
        [Column(TypeName="date")]
        public DateTime DueDate { get { return _DueDate; } set { _DueDate = value; } }

        private DateTime? _ReturnDate;
        [Column(TypeName="date")]
        [DisplayFormat(NullDisplayText="Not return")]
        public DateTime? ReturnDate { get { return _ReturnDate; } set { _ReturnDate = value; } }

        private short _RenewCount;
        [DefaultValue(0)]
        public short RenewCount { get { return _RenewCount; } set { _RenewCount = value; } }

        private Book _BorrowBook;
        public virtual Book BorrowBook { get { return _BorrowBook; } set { _BorrowBook = value; } }

        private Member _Borrower;
        public virtual Member Borrower { get { return _Borrower; } set { _Borrower = value; } }

    }
}