namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_NewRoles : DbMigration
    {
        public override void Up()
        {
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures ON");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(51,'Lock Receipt',8)");
            Sql("Insert into dbo.ScreenFeatures([Id],[Name],[ScreenId]) values(52,'Lock Receipts',11)");
            Sql("SET IDENTITY_INSERT dbo.ScreenFeatures OFF");
        }
        
        public override void Down()
        {
            Sql("delete from dbo.ScreenFeatures where Id in (51, 52)");
        }
    }
}
