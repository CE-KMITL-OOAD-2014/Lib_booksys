namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Books", "status2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "status2", c => c.String());
        }
    }
}
