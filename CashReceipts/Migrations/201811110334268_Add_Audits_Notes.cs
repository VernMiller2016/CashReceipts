namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Audits_Notes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Audits", "Notes", c => c.String());
            DropColumn("dbo.ReceiptHeaders", "DeleteReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReceiptHeaders", "DeleteReason", c => c.String());
            DropColumn("dbo.Audits", "Notes");
        }
    }
}
