using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;

namespace CashReceipts.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

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
            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Configure Asp Net Identity Tables
            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            //modelBuilder.Entity<IdentityUser>().Property(u => u.PasswordHash).HasMaxLength(500);
            //modelBuilder.Entity<IdentityUser>().Property(u => u.SecurityStamp).HasMaxLength(500);
            //modelBuilder.Entity<IdentityUser>().Property(u => u.PhoneNumber).HasMaxLength(50);

            //modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().HasKey(x => new { x.RoleId, x.UserId}).ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin>().HasKey(x=>x.UserId).ToTable("UserLogin");
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
        public DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public DbSet<PaymentMethod> TenderPaymentMethods { get; set; }
    }
}