namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScreensFeatures_Lookup : DbMigration
    {
        public override void Up()
        {
            Sql("Insert into dbo.Screens values(1,'Users Page Access',1)");
            Sql("Insert into dbo.Screens values(2,'Edit User Role',1)");
            Sql("Insert into dbo.Screens values(3,'Clerks Page Access',1)");
            Sql("Insert into dbo.Screens values(4,'Create Clerk',2)");
            Sql("Insert into dbo.Screens values(5,'Edit Clerk',2)");
            Sql("Insert into dbo.Screens values(6,'View Clerk Details',2)");
            Sql("Insert into dbo.Screens values(7,'Delete Clerk',2)");
            Sql("Insert into dbo.Screens values(8,'Entities Page Access',3)");
            Sql("Insert into dbo.Screens values(9,'Create Entity',3)");
            Sql("Insert into dbo.Screens values( 10,'Edit Entity',3)");
            Sql("Insert into dbo.Screens values( 11,'View Entity Details',3)");
            Sql("Insert into dbo.Screens values( 12,'Delete Entity',3)");
            Sql("Insert into dbo.Screens values( 13,'Department Page Access',4)");
            Sql("Insert into dbo.Screens values( 14,'Create Department',4)");
            Sql("Insert into dbo.Screens values( 15,'Edit Department',4)");
            Sql("Insert into dbo.Screens values( 16,'View Department Details',4)");
            Sql("Insert into dbo.Screens values( 17,'Delete Department',4)");
            Sql("Insert into dbo.Screens values( 18,'System Account Page Access',5)");
            Sql("Insert into dbo.Screens values( 19,'Edit System Account',5)");
            Sql("Insert into dbo.Screens values( 20,'View System Account Details',5)");
            Sql("Insert into dbo.Screens values( 21,'Delete System Account',5)");
            Sql("Insert into dbo.Screens values( 22,'Manage Receipts Page Access',8)");
            Sql("Insert into dbo.Screens values( 23,'Add New Receipt',8)");
            Sql("Insert into dbo.Screens values( 24,'Download Receipt',8)");
            Sql("Insert into dbo.Screens values(25, 'Post Receipt',8)");
            Sql("Insert into dbo.Screens values( 26,'Add New Receipt Item',8)");
            Sql("Insert into dbo.Screens values( 27,'Edit Receipt Item',8)");
            Sql("Insert into dbo.Screens values( 28,'Delete Receipt Item',8)");
            Sql("Insert into dbo.Screens values( 29,'Search Line Items Page Access',9)");
            Sql("Insert into dbo.Screens values( 30,'Export',9)");
            Sql("Insert into dbo.Screens values( 31,'Show Receipt',9)");
            Sql("Insert into dbo.Screens values( 32,'Receipts Export Page Access',10)");
            Sql("Insert into dbo.Screens values( 33,'Export Receipts',10)");
            Sql("Insert into dbo.Screens values( 34,'Export Line Items',10)");
            Sql("Insert into dbo.Screens values( 35,'Export Tenders',10)");
            Sql("Insert into dbo.Screens values( 36,'Export Receipts Details',10)");
            Sql("Insert into dbo.Screens values( 37,'Day Summary Report Page Access',11)");
            Sql("Insert into dbo.Screens values( 38,'Export and Print Summary',11)");
            Sql("Insert into dbo.Screens values( 39,'Audits Page Access',12)");
        }
        
        public override void Down()
        {
        }
    }
}
