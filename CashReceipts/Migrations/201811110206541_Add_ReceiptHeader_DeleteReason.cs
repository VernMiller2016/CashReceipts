namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ReceiptHeader_DeleteReason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReceiptHeaders", "DeleteReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReceiptHeaders", "DeleteReason");
        }
    }
}
