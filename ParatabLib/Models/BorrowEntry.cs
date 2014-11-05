using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ParatabLib.DataAccess;
namespace ParatabLib.Models
{
    //This class is representation of BorrowEntry
    public class BorrowEntry
    {
        private int _ID;
        [Key]
        public int ID { get { return _ID; } set { _ID = value; } }

        private int _UserID;
        public int UserID { get { return _UserID; } set { _UserID = value; } }

        private int _BookID;
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

        /* 4 below methods use to receive related borrowBook and borrower
         * via LibraryRepository object which can pass by reference or not pass parameter 
         * but instantiate in these method.
         */
        public Book GetBorrowBook()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.BookRepo.ListWhere(book => book.BookID == BookID).SingleOrDefault();
        }

        public Book GetBorrowBook(ref LibraryRepository libRepo)
        {
            return libRepo.BookRepo.ListWhere(book => book.BookID == BookID).SingleOrDefault();
        }

        public Member GetBorrower()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.MemberRepo.ListWhere(member => member.UserID == UserID).SingleOrDefault();
        }

        public Member GetBorrower(ref LibraryRepository libRepo)
        {
            return libRepo.MemberRepo.ListWhere(member => member.UserID == UserID).SingleOrDefault();
        }
    }
}