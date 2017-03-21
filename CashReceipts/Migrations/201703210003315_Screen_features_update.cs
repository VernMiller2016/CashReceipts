namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Screen_features_update : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.ScreenFeatures set name='Print Receipt' where id= 42");
            Sql("delete from dbo.ScreenFeatures where id in (43,47)");
        }
        
        public override void Down()
        {
        }
    }
}
