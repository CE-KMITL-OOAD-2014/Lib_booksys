namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TestLibrary.Models;
    internal sealed class Configuration : DbMigrationsConfiguration<TestLibrary.DataAccess.LibraryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TestLibrary.DataAccess.LibraryContext context)
        {
            News n = new News { Title = "WTF Surawit", Detail = "A simple detail", PostTime = DateTime.Now };
            context.NewsList.AddOrUpdate(target => target.Title);
            context.SaveChanges();
        }
    }
}
