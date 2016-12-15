namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Templates_Departments_DeleteAction : DbMigration
    {
        public override void Up()
        {
            Sql(@"

ALTER TABLE [dbo].[Templates]  Drop CONSTRAINT [FK_dbo.Templates_dbo.Departments_DepartmentID] 
go

ALTER TABLE [dbo].[Templates]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Templates_dbo.Departments_DepartmentID] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[Departments] ([DepartmentID]) on delete set null
GO

ALTER TABLE [dbo].[Templates] CHECK CONSTRAINT [FK_dbo.Templates_dbo.Departments_DepartmentID]
GO

");
        }
        
        public override void Down()
        {
        }
    }
}
