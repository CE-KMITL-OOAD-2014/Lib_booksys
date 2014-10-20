using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestLibrary.Models;
namespace TestLibrary.DataAccess
{
    public class LibraryRepository:IRepository
    {
        private LibraryContext context = new LibraryContext();
        private IGenericRepository<Book> bookRepository;
        private IGenericRepository<News> newsRepository;
        private IGenericRepository<BorrowEntry> borrowEntryRepository;
        private IGenericRepository<RequestEntry> requestEntryRepository;
        private IGenericRepository<Member> memberRepository;
        private IGenericRepository<Librarian> librarianRepository;


        public IGenericRepository<Book> BookRepo
        {
            get {
                if (bookRepository == null)
                    return bookRepository = new LocalRepository<Book>(context);
                else
                    return bookRepository;
            }
        }
        public IGenericRepository<News> NewsRepo
        {
            get
            {
                if (newsRepository == null)
                    return newsRepository = new LocalRepository<News>(context);
                else
                    return newsRepository;
            }
        }
        public IGenericRepository<BorrowEntry> BorrowEntryRepo
        {
            get
            {
                if (borrowEntryRepository == null)
                    return borrowEntryRepository = new LocalRepository<BorrowEntry>(context);
                else
                    return borrowEntryRepository;
            }
        }
        public IGenericRepository<RequestEntry> RequestEntryRepo
        {
            get
            {
                if (requestEntryRepository == null)
                    return requestEntryRepository = new LocalRepository<RequestEntry>(context);
                else
                    return requestEntryRepository;
            }
        }
        public IGenericRepository<Member> MemberRepo
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

        public void Save()
        {
            context.SaveChanges();
        }

    }
}