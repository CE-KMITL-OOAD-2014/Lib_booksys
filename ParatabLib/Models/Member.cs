using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParatabLib.DataAccess;
namespace ParatabLib.Models
{
    public class Member:Person
    {
        public override string Identify()
        {
            return ("Member "+this.UserName);
        }

        public List<BorrowEntry> GetRelatedBorrowEntry()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.BorrowEntryRepo.ListWhere(entry => entry.UserID == UserID);
        }

        public List<BorrowEntry> GetRelatedBorrowEntry(ref LibraryRepository libRepo)
        {
            return libRepo.BorrowEntryRepo.ListWhere(entry => entry.UserID == UserID);
        }

        public List<RequestEntry> GetRelatedRequestEntry()
        {
            LibraryRepository libRepo = new LibraryRepository();
            return libRepo.RequestEntryRepo.ListWhere(entry => entry.UserID == UserID);
        }

        public List<RequestEntry> GetRelatedRequestEntry(ref LibraryRepository libRepo)
        {
            return libRepo.RequestEntryRepo.ListWhere(entry => entry.UserID == UserID);
        }
    }
}