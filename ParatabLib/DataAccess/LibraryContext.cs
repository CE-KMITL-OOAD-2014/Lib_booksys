using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using ParatabLib.Models;
namespace ParatabLib.DataAccess
{
        public class LibraryContext : DbContext
        {
            public virtual DbSet<Librarian> Librarians { get; set; }
            public virtual DbSet<Member> Members { get; set; }
            public virtual DbSet<Book> Books { get; set; }
            public virtual DbSet<News> NewsList { get; set;}
            public virtual DbSet<BorrowEntry> BorrowList { get; set; }
            public virtual DbSet<RequestEntry> RequestList { get; set; }
        }
    
}