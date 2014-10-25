using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ParatabLib.DataAccess;
namespace ParatabLib.Models
{
    public class RequestEntry
    {

        private int _BookID;
        [Key,DatabaseGenerated(databaseGeneratedOption:DatabaseGeneratedOption.None)]
        public int BookID { get { return _BookID; } set { _BookID = value; } }

        private int _UserID;
        public int UserID { get { return _UserID; } set { _UserID = value; } }

        private DateTime _RequestDate;
        [Column(TypeName="date")]
        public DateTime RequestDate { get { return _RequestDate; } set { _RequestDate = value; } }

        private DateTime? _ExpireDate;
        [Column(TypeName="date")]
        public DateTime? ExpireDate { get { return _ExpireDate; } set { _ExpireDate = value; } }

        public Book GetRequestBook()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.BookRepo.ListWhere(book => book.BookID == BookID).SingleOrDefault();
        }

        public Member GetRequestUser()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.MemberRepo.ListWhere(member => member.UserID == UserID).SingleOrDefault();
        }
    }
}