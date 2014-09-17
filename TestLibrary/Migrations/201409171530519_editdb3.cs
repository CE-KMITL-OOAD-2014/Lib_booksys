namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editdb3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RequestEntries", "RequestDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.RequestEntries", "ExpireDate", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequestEntries", "ExpireDate", c => c.DateTime());
            AlterColumn("dbo.RequestEntries", "RequestDate", c => c.DateTime(nullable: false));
        }
    }
}
