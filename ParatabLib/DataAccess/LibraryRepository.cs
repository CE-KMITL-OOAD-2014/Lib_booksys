using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParatabLib.Models;
namespace ParatabLib.DataAccess
{
    /* Repository pattern implementation is here
     * by use same DatabaseContext Object ro synchronized data and update together
     */ 

    public class LibraryRepository
    {
        private LibraryContext context;
        private IGenericRepository<Book> bookRepository;
        private IGenericRepository<News> newsRepository;
        private IGenericRepository<BorrowEntry> borrowEntryRepository;
        private IGenericRepository<RequestEntry> requestEntryRepository;
        private IGenericRepository<Member> memberRepository;
        private IGenericRepository<Librarian> librarianRepository;

        public LibraryRepository(LibraryContext context)
        {
            this.context = context;
        }

        public LibraryRepository()
        {
            this.context = new LibraryContext();
        }

        public virtual IGenericRepository<Book> BookRepo
        {
            get {
                if (bookRepository == null)
                    return bookRepository = new LocalRepository<Book>(context);
                else
                    return bookRepository;
            }
        }
        public virtual IGenericRepository<News> NewsRepo
        {
            get
            {
                if (newsRepository == null)
                    return newsRepository = new LocalRepository<News>(context);
                else
                    return newsRepository;
            }
        }
        public virtual IGenericRepository<BorrowEntry> BorrowEntryRepo
        {
            get
            {
                if (borrowEntryRepository == null)
                    return borrowEntryRepository = new LocalRepository<BorrowEntry>(context);
                else
                    return borrowEntryRepository;
            }
        }
        public virtual IGenericRepository<RequestEntry> RequestEntryRepo
        {
            get
            {
                if (requestEntryRepository == null)
                    return requestEntryRepository = new LocalRepository<RequestEntry>(context);
                else
                    return requestEntryRepository;
            }
        }
        public virtual IGenericRepository<Member> MemberRepo
        {
            get
            {
                if (memberRepository == null)
                    return memberRepository = new LocalRepository<Member>(context);
                else
                    return memberRepository;
            }
        }
        public IGenericRepository<Librarian> LibrarianRepo
        {
            get
            {
                if (librarianRepository == null)
                    return librarianRepository = new LocalRepository<Librarian>(context);
                else
                    return librarianRepository;
            }
        }
        
        //This method use to save add/update/delete data by call context.SaveChanges() to operate in database.
        public void Save()
        {
            context.SaveChanges();
        }


    }
}