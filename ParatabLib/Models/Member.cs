using System;
using System.Collections.Generic;
using System.Linq;
using ParatabLib.DataAccess;
namespace ParatabLib.Models
{
    //This class is representation of Member which inherit from Person class
    public class Member:Person
    {
        public override string Identify()
        {
            return ("Member "+this.UserName);
        }

        /* 4 below methods use to receive related borrow/request entry of member.
         * via LibraryRepository object which can pass by reference or not pass parameter 
         * but instantiate in these method.
         */
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