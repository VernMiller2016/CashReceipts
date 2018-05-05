namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNew_TendersSearch_Screen : DbMigration
    {
        public override void Up()
        {
            Sql(@"SET IDENTITY_INSERT dbo.Screens ON 
            GO
            Insert into dbo.Screens(Id, Name) values(13, 'Search Tenders')
            SET IDENTITY_INSERT dbo.Screens OFF
            Go");

            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures ON");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(53,'Search Tenders Page Access',13)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(54,'Export',13)");
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures OFF");
        }
        
        public override void Down()
        {
        }
    }
}
