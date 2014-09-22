namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RequestEntries", "UserID", "dbo.Members");
            DropForeignKey("dbo.BorrowEntries", "UserID", "dbo.Members");
            DropPrimaryKey("dbo.Admins");
            DropPrimaryKey("dbo.Members");
            AlterColumn("dbo.Admins", "UserID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Admins", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "UserID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Members", "Email", c => c.String(nullable: false));
            AddPrimaryKey("dbo.Admins", "UserID");
            AddPrimaryKey("dbo.Members", "UserID");
            AddForeignKey("dbo.RequestEntries", "UserID", "dbo.Members", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.BorrowEntries", "UserID", "dbo.Members", "UserID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorrowEntries", "UserID", "dbo.Members");
            DropForeignKey("dbo.RequestEntries", "UserID", "dbo.Members");
            DropPrimaryKey("dbo.Members");
            DropPrimaryKey("dbo.Admins");
            AlterColumn("dbo.Members", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "UserID", c => c.Int(nullable: false));
            AlterColumn("dbo.Admins", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Admins", "UserID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Members", "UserID");
            AddPrimaryKey("dbo.Admins", "UserID");
            AddForeignKey("dbo.BorrowEntries", "UserID", "dbo.Members", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.RequestEntries", "UserID", "dbo.Members", "UserID", cascadeDelete: true);
        }
    }
}
