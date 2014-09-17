namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editborrowdb2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BorrowEntries", "DueDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.BorrowEntries", "ReturnDate", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BorrowEntries", "ReturnDate", c => c.DateTime());
            AlterColumn("dbo.BorrowEntries", "DueDate", c => c.DateTime(nullable: false));
        }
    }
}
