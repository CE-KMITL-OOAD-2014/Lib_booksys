namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "test", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "test");
        }
    }
}
