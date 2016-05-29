using CashReceipts.Models;
using System.Data.Entity.Migrations;

namespace CashReceipts
{
    public class ApplicationDbContextConfiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbContextConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            //This if for testing only
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}