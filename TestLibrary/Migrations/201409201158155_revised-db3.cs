namespace TestLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reviseddb3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Books", "testapi");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "testapi", c => c.String(maxLength: 8000, unicode: false));
        }
    }
}
