namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Templates_DescriptionLength : DbMigration
    {
        public override void Up()
        {
            Sql(@"Alter TABLE [dbo].[Templates]
Alter column [Description] [nvarchar](200) NOT NULL");
        }
        
        public override void Down()
        {
        }
    }
}
