namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_UserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "RoleId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ApplicationUsers", "RoleId");
            AddForeignKey("dbo.ApplicationUsers", "RoleId", "dbo.IdentityRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "RoleId", "dbo.IdentityRoles");
            DropIndex("dbo.ApplicationUsers", new[] { "RoleId" });
            DropColumn("dbo.ApplicationUsers", "RoleId");
        }
    }
}
