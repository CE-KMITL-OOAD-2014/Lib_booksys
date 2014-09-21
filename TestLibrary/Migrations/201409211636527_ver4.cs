namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ver4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RequestEntries", "BookID", "dbo.Books");
            DropPrimaryKey("dbo.RequestEntries");
            AddPrimaryKey("dbo.RequestEntries", "BookID");
            AddForeignKey("dbo.RequestEntries", "BookID", "dbo.Books", "BookID");
        }

        public override void Down()
        {
            AddColumn("dbo.RequestEntries", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.RequestEntries", "BookID", "dbo.Books");
            DropPrimaryKey("dbo.RequestEntries");
            AddPrimaryKey("dbo.RequestEntries", "ID");
            AddForeignKey("dbo.RequestEntries", "BookID", "dbo.Books", "BookID", cascadeDelete: true);
        }
    }
}