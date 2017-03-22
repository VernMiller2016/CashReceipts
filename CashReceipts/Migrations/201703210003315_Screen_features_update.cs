namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Screen_features_update : DbMigration
    {
        public override void Up()
        {
            Sql("delete from dbo.ScreenFeatures where id in (42, 43,47)");
        }
        
        public override void Down()
        {
        }
    }
}
