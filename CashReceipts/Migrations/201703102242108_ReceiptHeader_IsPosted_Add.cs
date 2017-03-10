namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReceiptHeader_IsPosted_Add : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReceiptHeaders", "IsPosted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReceiptHeaders", "IsPosted");
        }
    }
}
