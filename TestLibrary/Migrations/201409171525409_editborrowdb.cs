namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editborrowdb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BorrowEntries", "BorrowDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BorrowEntries", "BorrowDate", c => c.DateTime(nullable: false));
        }
    }
}
