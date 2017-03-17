namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleFeaturePermission_Add : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleFeaturePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.String(maxLength: 128),
                        FeatureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScreenFeatures", t => t.FeatureId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.FeatureId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleFeaturePermissions", "RoleId", "dbo.IdentityRoles");
            DropForeignKey("dbo.RoleFeaturePermissions", "FeatureId", "dbo.ScreenFeatures");
            DropIndex("dbo.RoleFeaturePermissions", new[] { "FeatureId" });
            DropIndex("dbo.RoleFeaturePermissions", new[] { "RoleId" });
            DropTable("dbo.RoleFeaturePermissions");
        }
    }
}
