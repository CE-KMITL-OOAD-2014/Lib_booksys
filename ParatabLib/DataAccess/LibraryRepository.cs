using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParatabLib.Models;
namespace ParatabLib.DataAccess
{
    public class LibraryRepository:IRepository
    {
        private LibraryContext context;
        private LocalRepository<Book> bookRepository;
        private LocalRepository<News> newsRepository;
        private LocalRepository<BorrowEntry> borrowEntryRepository;
        private LocalRepository<RequestEntry> requestEntryRepository;
        private LocalRepository<Member> memberRepository;
        private LocalRepository<Librarian> librarianRepository;

        public LibraryRepository(LibraryContext context)
        {
            this.context = context;
        }

        public LibraryRepository()
        {
            this.context = new LibraryContext();
        }

        public LocalRepository<Book> BookRepo
        {
            get {
                if (bookRepository == null)
                    return bookRepository = new LocalRepository<Book>(context);
                else
                    return bookRepository;
            }
        }
        public virtual LocalRepository<News> NewsRepo
        {
            get
            {
                if (newsRepository == null)
                    return newsRepository = new LocalRepository<News>(context);
                else
                    return newsRepository;
            }
        }
        public virtual LocalRepository<BorrowEntry> BorrowEntryRepo
        {
            get
            {
                if (borrowEntryRepository == null)
                    return borrowEntryRepository = new LocalRepository<BorrowEntry>(context);
                else
                    return borrowEntryRepository;
            }
        }
        public virtual LocalRepository<RequestEntry> RequestEntryRepo
        {
            get
            {
                if (requestEntryRepository == null)
                    return requestEntryRepository = new LocalRepository<RequestEntry>(context);
                else
                    return requestEntryRepository;
            }
        }
        public virtual LocalRepository<Member> MemberRepo
        {
            get
            {
                if (memberRepository == null)
                    return memberRepository = new LocalRepository<Member>(context);
                else
                    return memberRepository;
            }
        }
        public LocalRepository<Librarian> LibrarianRepo
        {
            get
            {
                if (librarianRepository == null)
                    return librarianRepository = new LocalRepository<Librarian>(context);
                else
                    return librarianRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

    }
}