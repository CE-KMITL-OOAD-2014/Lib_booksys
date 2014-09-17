namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editnewsdb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.News", "PostTime", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "PostTime", c => c.DateTime(nullable: false));
        }
    }
}
