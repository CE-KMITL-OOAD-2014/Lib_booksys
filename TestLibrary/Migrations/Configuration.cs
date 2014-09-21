namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TestLibrary.DataAccess.LibraryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TestLibrary.DataAccess.LibraryContext context)
        {
                
        }
    }
}
