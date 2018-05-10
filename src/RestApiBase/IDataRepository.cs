using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RestApiBase
{
    public interface IDataRepository
    {
        IdentityResult CreateUser(string userName, string password, string email, string roleName);
        ApplicationUser GetUser(string userName);
    }
    public static class DbContextExtensions
    {
        public static IdentityResult CreateUser(this DbContext context, string userName, string roleName,
            string email, string password)
        {
            return CreateUserAsync(context, userName, roleName, email, password).Result;
        }
        public static async Task<IdentityResult> CreateUserAsync(this DbContext context, string userName, string roleName, string email,
            string password)
        {
            //TODO:Check
            //if (context.Users.Any(u => u.UserName == userName))
            //    return IdentityResult.Failed("User with the same name already exists");
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            if (!roleStore.Roles.Any(r => r.Name.Equals(roleName)))
                await roleManager.CreateAsync(new IdentityRole() { Name = roleName });

            var userStore = new UserStore<IdentityUser>(context);
            var userManager = new UserManager<IdentityUser>(userStore);

            var user = new ApplicationUser() { UserName = userName, Email = email };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return IdentityResult.Failed(result.Errors.ToArray());
            var addToRoleRes = await userManager.AddToRoleAsync(user.Id, roleName);
            if (!addToRoleRes.Succeeded)
                return IdentityResult.Failed($"User:{user.Id} couldn't be added to role:{roleName}");
            return result;
        }
    }

}
