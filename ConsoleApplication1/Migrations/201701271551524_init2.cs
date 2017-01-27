namespace ConsoleApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AkkaStorageItems", "PayloadType", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AkkaStorageItems", "PayloadType");
        }
    }
}
