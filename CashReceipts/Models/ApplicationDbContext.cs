using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CashReceipts.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
#if DEBUG
            : base(Environment.MachineName, throwIfV1Schema: false)
#else
            : base("CashReceipts", throwIfV1Schema: false)
#endif
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReceiptHeader>()
                .HasRequired(c => c.Department)
                .WithMany()
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().HasKey(x => new { x.RoleId, x.UserId }).ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin>().HasKey(x => x.UserId).ToTable("UserLogin");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            //modelBuilder.Entity<IdentityUserClaim>().Property(u => u.ClaimType).HasMaxLength(150);
            //modelBuilder.Entity<IdentityUserClaim>().Property(u => u.ClaimValue).HasMaxLength(500);

            //modelBuilder.Configurations.Add(new ReceiptBodyMap());

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Clerk> Clerks { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<Tender> Tenders { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<ReceiptHeader> ReceiptHeaders { get; set; }
        public DbSet<ReceiptBody> ReceiptBodies { get; set; }
        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        public DbSet<PaymentMethod> TenderPaymentMethods { get; set; }

        public List<Template> GetGCAccounts(ColumnOrders colIndex, string searchTerm, int rowsNum, int skipRows = 0, bool getDistinct = true)
        {
            var indexParam = new SqlParameter("@index", SqlDbType.Int) { Value = (getDistinct ? colIndex : 10 + colIndex) };
            var resultsCountParam = new SqlParameter("@resultsCount", SqlDbType.Int) { Value = rowsNum };
            var skipRowsParam = new SqlParameter("@skipRows", SqlDbType.Int) { Value = skipRows };
            var searchTermParam = new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = searchTerm };

            return this.Database.SqlQuery<Template>("SearchAccounts @index, @searchTerm, @resultsCount, @skipRows", indexParam,
                searchTermParam, resultsCountParam, skipRowsParam).ToList();
        }
    }
}