namespace CashReceipts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissing : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Audits",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            OperationType = c.Int(nullable: false),
            //            EntityType = c.Int(nullable: false),
            //            EntityId = c.String(nullable: false),
            //            UserId = c.String(nullable: false),
            //            ActionDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Clerks",
            //    c => new
            //        {
            //            ClerkID = c.Int(nullable: false, identity: true),
            //            FirstName = c.String(nullable: false, maxLength: 25),
            //            LastName = c.String(nullable: false, maxLength: 25),
            //            UserId = c.String(maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.ClerkID)
            //    .ForeignKey("dbo.ApplicationUsers", t => t.UserId)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.ReceiptHeaders",
            //    c => new
            //        {
            //            ReceiptHeaderID = c.Int(nullable: false, identity: true),
            //            ReceiptDate = c.DateTime(nullable: false),
            //            ReceiptTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ReceiptNumber = c.Int(nullable: false),
            //            ClerkID = c.Int(nullable: false),
            //            DepartmentID = c.Int(nullable: false),
            //            Comments = c.String(maxLength: 250),
            //            IsDeleted = c.Boolean(nullable: false),
            //            ReceivedFor = c.String(maxLength: 250),
            //            IsPosted = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ReceiptHeaderID)
            //    .ForeignKey("dbo.Clerks", t => t.ClerkID, cascadeDelete: true)
            //    .ForeignKey("dbo.Departments", t => t.DepartmentID)
            //    .Index(t => t.ClerkID)
            //    .Index(t => t.DepartmentID);
            
            //CreateTable(
            //    "dbo.Departments",
            //    c => new
            //        {
            //            DepartmentID = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false, maxLength: 40),
            //        })
            //    .PrimaryKey(t => t.DepartmentID);
            
            //CreateTable(
            //    "dbo.Templates",
            //    c => new
            //        {
            //            TemplateID = c.Int(nullable: false, identity: true),
            //            Fund = c.String(nullable: false, maxLength: 3),
            //            Dept = c.String(nullable: false, maxLength: 3),
            //            Program = c.String(maxLength: 2),
            //            Project = c.String(maxLength: 4),
            //            BaseElementObjectDetail = c.String(maxLength: 9),
            //            Description = c.String(nullable: false, maxLength: 200),
            //            DepartmentID = c.Int(),
            //            Order = c.Int(nullable: false),
            //            DataSource = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.TemplateID)
            //    .ForeignKey("dbo.Departments", t => t.DepartmentID)
            //    .Index(t => t.DepartmentID);
            
            //CreateTable(
            //    "dbo.ReceiptBodies",
            //    c => new
            //        {
            //            ReceiptBodyID = c.Int(nullable: false, identity: true),
            //            LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ReceiptHeaderID = c.Int(nullable: false),
            //            TemplateID = c.Int(nullable: false),
            //            AccountDescription = c.String(),
            //        })
            //    .PrimaryKey(t => t.ReceiptBodyID)
            //    .ForeignKey("dbo.ReceiptHeaders", t => t.ReceiptHeaderID, cascadeDelete: true)
            //    .ForeignKey("dbo.Templates", t => t.TemplateID, cascadeDelete: true)
            //    .Index(t => t.ReceiptHeaderID)
            //    .Index(t => t.TemplateID);
            
            //CreateTable(
            //    "dbo.Tenders",
            //    c => new
            //        {
            //            TenderID = c.Int(nullable: false, identity: true),
            //            Description = c.String(maxLength: 30),
            //            Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ReceiptHeaderID = c.Int(nullable: false),
            //            PaymentMethodId = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.TenderID)
            //    .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodId, cascadeDelete: true)
            //    .ForeignKey("dbo.ReceiptHeaders", t => t.ReceiptHeaderID, cascadeDelete: true)
            //    .Index(t => t.ReceiptHeaderID)
            //    .Index(t => t.PaymentMethodId);
            
            //CreateTable(
            //    "dbo.PaymentMethods",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            CUMTH = c.String(),
            //            Name = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.ApplicationUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.IdentityUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //            ApplicationUser_Id = c.String(maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
            //    .Index(t => t.ApplicationUser_Id);
            
            //CreateTable(
            //    "dbo.UserLogin",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            LoginProvider = c.String(),
            //            ProviderKey = c.String(),
            //            ApplicationUser_Id = c.String(maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.UserId)
            //    .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
            //    .Index(t => t.ApplicationUser_Id);
            
            //CreateTable(
            //    "dbo.UserRole",
            //    c => new
            //        {
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ApplicationUser_Id = c.String(maxLength: 128),
            //            IdentityRole_Id = c.String(maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.RoleId, t.UserId })
            //    .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
            //    .ForeignKey("dbo.IdentityRoles", t => t.IdentityRole_Id)
            //    .Index(t => t.ApplicationUser_Id)
            //    .Index(t => t.IdentityRole_Id);
            
            //CreateTable(
            //    "dbo.Entities",
            //    c => new
            //        {
            //            EntityID = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false, maxLength: 40),
            //            Address1 = c.String(maxLength: 35),
            //            Address2 = c.String(maxLength: 35),
            //            City = c.String(maxLength: 30),
            //            State = c.String(maxLength: 2),
            //            ZipCode = c.String(maxLength: 10),
            //            Telephone = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.EntityID);
            
            //CreateTable(
            //    "dbo.GlobalSettings",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Key = c.String(),
            //            Value = c.String(),
            //            RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.IdentityRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.ScreenFeatures",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //            ScreenId = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Screens", t => t.ScreenId, cascadeDelete: true)
            //    .Index(t => t.ScreenId);
            
            //CreateTable(
            //    "dbo.Screens",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.ScreenFeatures", "ScreenId", "dbo.Screens");
            //DropForeignKey("dbo.UserRole", "IdentityRole_Id", "dbo.IdentityRoles");
            //DropForeignKey("dbo.Clerks", "UserId", "dbo.ApplicationUsers");
            //DropForeignKey("dbo.UserRole", "ApplicationUser_Id", "dbo.ApplicationUsers");
            //DropForeignKey("dbo.UserLogin", "ApplicationUser_Id", "dbo.ApplicationUsers");
            //DropForeignKey("dbo.IdentityUserClaims", "ApplicationUser_Id", "dbo.ApplicationUsers");
            //DropForeignKey("dbo.Tenders", "ReceiptHeaderID", "dbo.ReceiptHeaders");
            //DropForeignKey("dbo.Tenders", "PaymentMethodId", "dbo.PaymentMethods");
            //DropForeignKey("dbo.ReceiptBodies", "TemplateID", "dbo.Templates");
            //DropForeignKey("dbo.ReceiptBodies", "ReceiptHeaderID", "dbo.ReceiptHeaders");
            //DropForeignKey("dbo.ReceiptHeaders", "DepartmentID", "dbo.Departments");
            //DropForeignKey("dbo.Templates", "DepartmentID", "dbo.Departments");
            //DropForeignKey("dbo.ReceiptHeaders", "ClerkID", "dbo.Clerks");
            //DropIndex("dbo.ScreenFeatures", new[] { "ScreenId" });
            //DropIndex("dbo.UserRole", new[] { "IdentityRole_Id" });
            //DropIndex("dbo.UserRole", new[] { "ApplicationUser_Id" });
            //DropIndex("dbo.UserLogin", new[] { "ApplicationUser_Id" });
            //DropIndex("dbo.IdentityUserClaims", new[] { "ApplicationUser_Id" });
            //DropIndex("dbo.Tenders", new[] { "PaymentMethodId" });
            //DropIndex("dbo.Tenders", new[] { "ReceiptHeaderID" });
            //DropIndex("dbo.ReceiptBodies", new[] { "TemplateID" });
            //DropIndex("dbo.ReceiptBodies", new[] { "ReceiptHeaderID" });
            //DropIndex("dbo.Templates", new[] { "DepartmentID" });
            //DropIndex("dbo.ReceiptHeaders", new[] { "DepartmentID" });
            //DropIndex("dbo.ReceiptHeaders", new[] { "ClerkID" });
            //DropIndex("dbo.Clerks", new[] { "UserId" });
            //DropTable("dbo.Screens");
            //DropTable("dbo.ScreenFeatures");
            //DropTable("dbo.IdentityRoles");
            //DropTable("dbo.GlobalSettings");
            //DropTable("dbo.Entities");
            //DropTable("dbo.UserRole");
            //DropTable("dbo.UserLogin");
            //DropTable("dbo.IdentityUserClaims");
            //DropTable("dbo.ApplicationUsers");
            //DropTable("dbo.PaymentMethods");
            //DropTable("dbo.Tenders");
            //DropTable("dbo.ReceiptBodies");
            //DropTable("dbo.Templates");
            //DropTable("dbo.Departments");
            //DropTable("dbo.ReceiptHeaders");
            //DropTable("dbo.Clerks");
            //DropTable("dbo.Audits");
        }
    }
}
