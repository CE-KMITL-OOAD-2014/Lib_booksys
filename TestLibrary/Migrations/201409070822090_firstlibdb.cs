namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstlibdb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Books", "Year", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Books", "Year", c => c.Int(nullable: false));
        }
    }
}
