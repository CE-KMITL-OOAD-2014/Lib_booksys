using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using ParatabLib.DataAccess;
namespace ParatabLib.Models
{
    public enum Status
    {
        Available,Borrowed,Reserved,Lost
    }
    public class Book
    {
        private int _BookID;
        [Key]
        public int BookID { get { return _BookID; } set { _BookID = value; } }

        private string _CallNumber;
        [Required(ErrorMessage="Call number is required.")]
        public string CallNumber { get { return _CallNumber; } set { _CallNumber = value; } }

        private string _BookName;
        [Required(ErrorMessage="Book name is required.")]
        public string BookName { get { return _BookName; } set { _BookName = value; } }


        private string _Author;
        [DisplayFormat(NullDisplayText="Unknown author.")]
        public string Author { get { return _Author; } set { _Author = value; } }

        private string _Detail;
        [DisplayFormat(NullDisplayText="Unknown detail.")]
        public string Detail { get { return _Detail; } set { _Detail = value; } }

        private int? _Year;
        [DisplayFormat(NullDisplayText="Unknown year.")]
        public int? Year { get { return _Year; } set { _Year = value; } }

        private string _Publisher;
        [DisplayFormat(NullDisplayText="Unknown publisher.")]
        public string Publisher { get { return _Publisher; } set { _Publisher = value; } }

        private Status _BookStatus;
        [DefaultValue(Status.Available)]
        [Column("Status")]
        public Status BookStatus { get { return _BookStatus; } set { _BookStatus = value; } }

        public List<BorrowEntry> GetRelatedBorrowEntry()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.BorrowEntryRepo.ListWhere(entry => entry.BookID == BookID);
        }

        public RequestEntry GetRelatedRequestEntry()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.RequestEntryRepo.ListWhere(entry => entry.BookID == BookID).SingleOrDefault();
        }
    }
}