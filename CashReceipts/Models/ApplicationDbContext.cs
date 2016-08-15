using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using CashReceipts.Helpers;
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
        public DbSet<Audit> Audits { get; set; }

        public List<Template> GetGCAccounts(ColumnOrders colIndex, string searchTerm, int rowsNum, int skipRows = 0)
        {
            var indexParam = new SqlParameter("@index", SqlDbType.Int) { Value = colIndex };
            var resultsCountParam = new SqlParameter("@resultsCount", SqlDbType.Int) { Value = rowsNum };
            var skipRowsParam = new SqlParameter("@skipRows", SqlDbType.Int) { Value = skipRows };
            var searchTermParam = new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = searchTerm };

            return this.Database.SqlQuery<Template>("SearchAccounts @index, @searchTerm, @resultsCount, @skipRows", indexParam,
                searchTermParam, resultsCountParam, skipRowsParam).ToList();
        }

        public List<Template> FilterGlAccounts(int? skip, int? take, SearchAccountDataSource entityType, string fund, string dept, string program, string project,
            string baseElementObjectDetail, string description, int? acctIndex, ref int accountsValidResultsCount)
        {
            List<Template> accounts = new List<Template>();
            if (take.HasValue)
            {
                var command = Database.Connection.CreateCommand();
                command.CommandTimeout = 0;
                command.CommandText =
                    "SearchGLAccounts @Fund, @Dept, @Program, @Project, @BaseElementObjectDetail, @Description, @ACTINDX, @resultsCount, @skipRows, @entityType";
                command.Parameters.Add(new SqlParameter("@Fund", SqlDbType.NVarChar) { Value = GetDbValue(fund) });
                command.Parameters.Add(new SqlParameter("@Dept", SqlDbType.NVarChar) { Value = GetDbValue(dept) });
                command.Parameters.Add(new SqlParameter("@Program", SqlDbType.NVarChar) { Value = GetDbValue(program) });
                command.Parameters.Add(new SqlParameter("@Project", SqlDbType.NVarChar) { Value = GetDbValue(project) });
                command.Parameters.Add(new SqlParameter("@BaseElementObjectDetail", SqlDbType.NVarChar)
                {
                    Value = GetDbValue(baseElementObjectDetail)
                });
                command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = GetDbValue(description) });
                command.Parameters.Add(new SqlParameter("@ACTINDX", SqlDbType.Int) { Value = acctIndex.HasValue ? acctIndex : (object)DBNull.Value });
                command.Parameters.Add(new SqlParameter("@skipRows", SqlDbType.Int) { Value = skip });
                command.Parameters.Add(new SqlParameter("@entityType", SqlDbType.Int) { Value = (int)entityType });
                command.Parameters.Add(new SqlParameter("@resultsCount", SqlDbType.Int) { Value = take });

                try
                {
                    Database.Connection.Open();
                    var reader = command.ExecuteReader();
                    var objectContext = ((IObjectContextAdapter)this).ObjectContext;
                    accounts = objectContext.Translate<Template>(reader).ToList();
                    reader.NextResult();
                    accountsValidResultsCount = objectContext.Translate<int>(reader).First();
                }
                finally
                {
                    Database.Connection.Close();
                }
            }
            return accounts;
        }

        private object GetDbValue(string colValue)
        {
            return string.IsNullOrEmpty(colValue) ? DBNull.Value : (object)colValue;
        }

        public bool IsValidGCAccount(Template template)
        {
            var dbName = new LookupHelper(this).GcDbName;
            var sqlQuery = $"Select count(*) from {dbName}dbo.GL00100 where Active = 1 and [ACTNUMBR_1] = '{template.Fund}'"
                + $" and [ACTNUMBR_2] = '{template.Dept}' and [ACTNUMBR_3] = '{template.Program}' and [ACTNUMBR_4] = '{template.Project}'"
                + $" and [ACTNUMBR_5] = '{template.BaseElementObjectDetail}'";
            return this.Database.SqlQuery<int>(sqlQuery).First() > 0;
        }

        public bool IsValidDistAccount(Template template)
        {
            var dbName = new LookupHelper(this).DistDbName;
            var sqlQuery = $"Select count(*) from {dbName}dbo.GL00100 where Active = 1 "
                + $"and [ACTNUMBR_1] = '{template.Fund}{template.Dept}{template.Program}'"
                + $" and [ACTNUMBR_2] = '{template.Project}'"
                + $" and [ACTNUMBR_3] = '{template.BaseElementObjectDetail}'";
            return this.Database.SqlQuery<int>(sqlQuery).First() > 0;
        }

        public Template GetRemoteAccount(int accountIndex, AccountDataSource accountDataSource)
        {
            var acctDs = accountDataSource == AccountDataSource.GrantCounty
                ? SearchAccountDataSource.GrantCounty
                : SearchAccountDataSource.District;
            int resultsCount = 0;
            var result = FilterGlAccounts(0, 1, acctDs, "", "", "", "", "", "", accountIndex, ref resultsCount);
            return result.FirstOrDefault();
        }
    }
}