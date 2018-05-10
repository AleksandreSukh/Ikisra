using System;
using System.Data.Entity;
using System.Linq;
using ProjectAPI.Controllers;
using RestApiBase;

namespace ProjectAPI.DataSource
{
    public interface ISourceDbContext : IDisposable
    {
        DbSet<Statement> Statements { get; }
        IQueryable<ApplicationUser> UsersQueriable { get; }
        int SaveChanges();
        DbContext DbContext { get; }
    }
}