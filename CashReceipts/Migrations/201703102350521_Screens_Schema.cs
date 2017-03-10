namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Screens_Schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScreenFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ScreenId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Screens", t => t.ScreenId, cascadeDelete: true)
                .Index(t => t.ScreenId);
            
            CreateTable(
                "dbo.Screens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScreenFeatures", "ScreenId", "dbo.Screens");
            DropIndex("dbo.ScreenFeatures", new[] { "ScreenId" });
            DropTable("dbo.Screens");
            DropTable("dbo.ScreenFeatures");
        }
    }
}
