namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requestentry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RequestEntries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        BookID = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Books", t => t.BookID, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.BookID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequestEntries", "UserID", "dbo.Members");
            DropForeignKey("dbo.RequestEntries", "BookID", "dbo.Books");
            DropIndex("dbo.RequestEntries", new[] { "BookID" });
            DropIndex("dbo.RequestEntries", new[] { "UserID" });
            DropTable("dbo.RequestEntries");
        }
    }
}
