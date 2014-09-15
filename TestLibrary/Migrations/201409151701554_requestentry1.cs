namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requestentry1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RequestEntries", "ExpireDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequestEntries", "ExpireDate", c => c.DateTime(nullable: false));
        }
    }
}
