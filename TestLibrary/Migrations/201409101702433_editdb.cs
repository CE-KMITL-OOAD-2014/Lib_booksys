namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editdb : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Admins");
            DropPrimaryKey("dbo.Members");
            RenameColumn("dbo.Admins","Id","UserID");
            RenameColumn("dbo.Members","Id","UserID");
            AddPrimaryKey("dbo.Admins", "UserID");
            AddPrimaryKey("dbo.Members", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Admins", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Members");
            DropPrimaryKey("dbo.Admins");
            DropColumn("dbo.Members", "UserID");
            DropColumn("dbo.Admins", "UserID");
            AddPrimaryKey("dbo.Members", "Id");
            AddPrimaryKey("dbo.Admins", "Id");
        }
    }
}
