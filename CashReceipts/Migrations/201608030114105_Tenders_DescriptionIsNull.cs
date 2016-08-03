namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tenders_DescriptionIsNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tenders", "Description", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tenders", "Description", c => c.String(nullable: false, maxLength: 30));
        }
    }
}
