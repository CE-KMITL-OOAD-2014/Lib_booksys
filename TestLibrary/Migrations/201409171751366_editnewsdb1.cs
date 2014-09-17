namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editnewsdb1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.News", "PostTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.News", "PostTime", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
