using System.Data.Entity.Migrations;
using CashReceipts.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Migrations
{
    public class ApplicationDbContextConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbContextConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            context.Roles.AddOrUpdate(role => role.Id,
                new IdentityRole { Id = "6eae8487-db3e-46af-af20-1a63307ae86c", Name = "Administrators" },
                new IdentityRole { Id = "e90dc62d-8000-4b33-b299-b233425777fc", Name = "Clerks" });

            context.SaveChanges();
        }
    }
}