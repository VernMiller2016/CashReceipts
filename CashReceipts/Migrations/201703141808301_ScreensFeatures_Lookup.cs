namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScreensFeatures_Lookup : DbMigration
    {
        public override void Up()
        {
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures ON");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(1,'Users Page Access',1)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(2,'Edit User Role',1)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(3,'Clerks Page Access',2)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(4,'Create Clerk',2)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(5,'Edit Clerk',2)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(6,'View Clerk Details',2)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(7,'Delete Clerk',2)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(8,'Entities Page Access',3)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(9,'Create Entity',3)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 10,'Edit Entity',3)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 11,'View Entity Details',3)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 12,'Delete Entity',3)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 13,'Department Page Access',4)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 14,'Create Department',4)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 15,'Edit Department',4)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 16,'View Department Details',4)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 17,'Delete Department',4)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 18,'System Account Page Access',5)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 19,'Edit System Account',5)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 20,'View System Account Details',5)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 21,'Delete System Account',5)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 22,'Manage Receipts Page Access',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 23,'Add New Receipt',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 24,'Download Receipt',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(25, 'Post Receipt',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 26,'Add New Receipt Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 27,'Edit Receipt Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 28,'Delete Receipt Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 29,'Search Line Items Page Access',9)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 30,'Export',9)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 31,'Show Receipt',9)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 32,'Receipts Export Page Access',10)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 33,'Export Receipts',10)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 34,'Export Line Items',10)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 35,'Export Tenders',10)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 36,'Export Receipts Details',10)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 37,'Day Summary Report Page Access',11)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 38,'Export and Print Summary',11)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 39,'Audits Page Access',12)");
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures OFF");
        }
        
        public override void Down()
        {
        }
    }
}
