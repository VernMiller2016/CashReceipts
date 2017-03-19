namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScreenFeatures_Add_Missing : DbMigration
    {
        public override void Up()
        {
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures ON");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 40,'Grant County Accounts Page Access',6)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 41,'District Accounts Page Access',7)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 42,'Read Receipt Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 43,'Read Tender Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 44,'Add Tender Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 45,'Edit Tender Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 46,'Delete Tender Item',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 47,'Read Receipt Body',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 48,'Add Receipt Body',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 49,'Edit Receipt Body',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values( 50,'Delete Receipt Body',8)");
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures OFF");
        }

        public override void Down()
        {
        }
    }
}
