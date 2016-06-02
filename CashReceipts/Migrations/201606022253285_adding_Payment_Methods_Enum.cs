namespace CashReceipts
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adding_Payment_Methods_Enum : DbMigration
    {
        public override void Up()
        {
            Sql(@"SET IDENTITY_INSERT [dbo].[PaymentMethods] ON 
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (1, N'A', N'NSF')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (2, N'1', N'Check')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (3, N'2', N'Cash')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (4, N'3', N'Money Order')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (5, N'4', N'Warrant')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (6, N'5', N'See Terrascan')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (7, N'6', N'EFT')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (8, N'7', N'Refund')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (9, N'8', N'Over/Short')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (10, N'9', N'Cash Back')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (11, N'I', N'Investments')
                GO
                INSERT [dbo].[PaymentMethods] ([Id], [CUMTH], [Name]) VALUES (12, N'B', N'To Be Billed')
                GO
                SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF");
        }
        
        public override void Down()
        {
        }
    }
}
