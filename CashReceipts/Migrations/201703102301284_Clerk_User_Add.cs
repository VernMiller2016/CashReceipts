namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Clerk_User_Add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clerks", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Clerks", "UserId");
            AddForeignKey("dbo.Clerks", "UserId", "dbo.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clerks", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Clerks", new[] { "UserId" });
            DropColumn("dbo.Clerks", "UserId");
        }
    }
}
