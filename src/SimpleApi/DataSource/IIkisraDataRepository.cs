using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using ProjectAPI.Controllers;
using RestApiBase;

namespace ProjectAPI.DataSource
{
    public interface IIkisraDataRepository : IDataRepository, IDisposable
    {
        Dictionary<string, Statement> GetAllStatements();
        void AddNewStatement(Statement newStatement);
        void Save();
    }

    public class IkisraDataRepository : IIkisraDataRepository
    {
        private readonly ISourceDbContext _context;

        public IkisraDataRepository(Func<ISourceDbContext> contextProvider)
        {
            this._context = contextProvider.Invoke();
        }
        public IkisraDataRepository(ISourceDbContext context)
        {
            this._context = context;
        }

        #region Queries

        public ApplicationUser GetUser(string userName) => _context.UsersQueriable.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));

       
        //TODO:
        private static Dictionary<string, Statement> cachedStatements = null;

        #endregion

        #region Commands

        public Dictionary<string, Statement> GetAllStatements()
        {
            if (cachedStatements == null)
                cachedStatements = _context.Statements.ToDictionary(w => w.Name, w => w);
            return cachedStatements;
        }

        public void AddNewStatement(Statement newStatement) => _context.Statements.Add(newStatement);

        public void Save() => _context.SaveChanges();


        public IdentityResult CreateUser(string userName, string password, string email, string roleName) => _context.DbContext.CreateUser(
            userName: userName,
            password: password,
            roleName: roleName,
            email: email);

        public void Dispose() => _context?.Dispose();

        #endregion
    }
}
