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
            public DbSet<Admin> Admins { get; set; }
            public DbSet<Member> Members { get; set; }
            public DbSet<Book> Books { get; set; }
            public DbSet<News> NewsList { get; set; }
        }
    
}