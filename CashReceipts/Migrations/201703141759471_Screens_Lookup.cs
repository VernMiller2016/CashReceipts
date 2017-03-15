namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Screens_Lookup : DbMigration
    {
        public override void Up()
        {
            Sql("Insert into dbo.Screens values(1,'Users')");
            Sql("Insert into dbo.Screens values( 2,'Clerks')");
            Sql("Insert into dbo.Screens values(3, 'Entity')");
            Sql("Insert into dbo.Screens values(4,'Departments')");
            Sql("Insert into dbo.Screens values(5,'System Accounts')");
            Sql("Insert into dbo.Screens values(6,'Grant County Accounts')");
            Sql("Insert into dbo.Screens values(7,'District Accounts')");
            Sql("Insert into dbo.Screens values(8,'Manage Receipts')");
            Sql("Insert into dbo.Screens values(9,'Search Line Items')");
            Sql("Insert into dbo.Screens values(10, 'Receipts Export')");
            Sql("Insert into dbo.Screens values(11, 'Day Summary Report')");
            Sql("Insert into dbo.Screens values(12, 'Audits')");
        }
        
        public override void Down()
        {
        }
    }
}
