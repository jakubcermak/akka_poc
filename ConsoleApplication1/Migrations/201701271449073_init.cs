namespace ConsoleApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AkkaStorageItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersistenceId = c.String(maxLength: 300),
                        SequenceNr = c.Long(nullable: false),
                        Manifest = c.String(),
                        PayloadJson = c.String(),
                        Path = c.String(),
                        WriterGuid = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PersistenceId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.AkkaStorageItems", new[] { "PersistenceId" });
            DropTable("dbo.AkkaStorageItems");
        }
    }
}
