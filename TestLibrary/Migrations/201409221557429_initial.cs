namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookID = c.Int(nullable: false, identity: true),
                        BookName = c.String(nullable: false),
                        Author = c.String(),
                        Detail = c.String(),
                        Year = c.Int(),
                        Publisher = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookID);
            
            CreateTable(
                "dbo.BorrowEntries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        BookID = c.Int(nullable: false),
                        BorrowDate = c.DateTime(nullable: false, storeType: "date"),
                        DueDate = c.DateTime(nullable: false, storeType: "date"),
                        ReturnDate = c.DateTime(storeType: "date"),
                        RenewCount = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Books", t => t.BookID, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.BookID);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.RequestEntries",
                c => new
                    {
                        BookID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        RequestDate = c.DateTime(nullable: false, storeType: "date"),
                        ExpireDate = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.BookID)
                .ForeignKey("dbo.Books", t => t.BookID)
                .ForeignKey("dbo.Members", t => t.UserID, cascadeDelete: true)
                .Index(t => t.BookID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        NewsID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Detail = c.String(nullable: false),
                        PostTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NewsID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorrowEntries", "UserID", "dbo.Members");
            DropForeignKey("dbo.RequestEntries", "UserID", "dbo.Members");
            DropForeignKey("dbo.RequestEntries", "BookID", "dbo.Books");
            DropForeignKey("dbo.BorrowEntries", "BookID", "dbo.Books");
            DropIndex("dbo.RequestEntries", new[] { "UserID" });
            DropIndex("dbo.RequestEntries", new[] { "BookID" });
            DropIndex("dbo.BorrowEntries", new[] { "BookID" });
            DropIndex("dbo.BorrowEntries", new[] { "UserID" });
            DropTable("dbo.News");
            DropTable("dbo.RequestEntries");
            DropTable("dbo.Members");
            DropTable("dbo.BorrowEntries");
            DropTable("dbo.Books");
            DropTable("dbo.Admins");
        }
    }
}
