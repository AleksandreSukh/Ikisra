using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using ProjectAPI.Controllers;
using RestApiBase;
using SharedTemplate;

namespace ProjectAPI.DataSource
{
    public class SourceDbContextBase : IdentityDbContext<ApplicationUser>, ISourceDbContext
    {
        public SourceDbContextBase(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetupEntity(modelBuilder);
        }
        void SetupEntity(DbModelBuilder modelBuilder)
        {
            // Change the name of the table to be Users instead of AspNetUsers
            modelBuilder.Entity<IdentityUser>()
                .ToTable($"{RoleNames.Users}");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable($"{RoleNames.Users}");

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }

        public DbSet<Statement> Statements { get; set; }
        public IQueryable<ApplicationUser> UsersQueriable => Users;
        public DbContext DbContext => this;

    }

    public class SourceDbContextMssql : SourceDbContextBase
    {
        //For migrations
        public SourceDbContextMssql() : base(DependencyRepository.ConnectionStringName)
        {
        }
        public SourceDbContextMssql(string connectionString)
            : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SourceDbContextMssql>());
            base.OnModelCreating(modelBuilder);
        }
    }
    public class SourceDbContextPostgres : SourceDbContextBase
    {
        //For migrations
        public SourceDbContextPostgres() : base(DependencyRepository.ConnectionStringName)
        {
        }

        public SourceDbContextPostgres(string connectionString)
            : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SourceDbContextPostgres>());
            base.OnModelCreating(modelBuilder);
        }
    }


}
