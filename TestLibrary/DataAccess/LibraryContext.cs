using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TestLibrary.Models;
namespace TestLibrary.DataAccess
{
        public class LibraryContext : DbContext
        {
            public DbSet<Librarian> Librarians { get; set; }
            public DbSet<Member> Members { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<News> NewsList { get; set; }
            public DbSet<BorrowEntry> BorrowList { get; set; }
            public DbSet<RequestEntry> RequestList { get; set; }
        }
    
}