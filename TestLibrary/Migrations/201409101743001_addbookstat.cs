namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbookstat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "Status");
        }
    }
}
