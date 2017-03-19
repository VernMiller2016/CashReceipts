namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Screens_Lookup : DbMigration
    {
        public override void Up()
        {
            Sql(@"SET IDENTITY_INSERT dbo.Screens ON 
            GO
            Insert into dbo.Screens(Id, Name) values(1,'Users')
            Insert into dbo.Screens(Id, Name) values(2,'Clerks')
            Insert into dbo.Screens(Id, Name) values(3, 'Entity')
            Insert into dbo.Screens(Id, Name) values(4,'Departments')
            Insert into dbo.Screens(Id, Name) values(5,'System Accounts')
            Insert into dbo.Screens(Id, Name) values(6,'Grant County Accounts')
            Insert into dbo.Screens(Id, Name) values(7,'District Accounts')
            Insert into dbo.Screens(Id, Name) values(8,'Manage Receipts')
            Insert into dbo.Screens(Id, Name) values(9,'Search Line Items')
            Insert into dbo.Screens(Id, Name) values(10, 'Receipts Export')
            Insert into dbo.Screens(Id, Name) values(11, 'Day Summary Report')
            Insert into dbo.Screens(Id, Name) values(12, 'Audits')
            SET IDENTITY_INSERT dbo.Screens OFF
            Go");
        }

        public override void Down()
        {
        }
    }
}
