using Microsoft.AspNetCore.Identity;
using MyEmailWebApi.Constants;
using MyEmailWebApi.Data;

namespace MyEmailWebApi
{
    public class Seeder
    {
        private readonly UserContext _userContext;

        public Seeder(UserContext userContext)
        {
            _userContext = userContext;
        }

        public void SeedRoles()
        {
            var expectedRoleNames = new string[] { RoleNames.EMPLOYEE, RoleNames.MANAGER, RoleNames.ADMIN };
            var dbRoleNames = _userContext.Roles.Where((role) => expectedRoleNames.Contains(role.Name)).Select((role) => role.Name).ToArray();
            bool anyAddition = false;
            if (!dbRoleNames.Any((role) => role == RoleNames.EMPLOYEE))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.EMPLOYEE,
                    NormalizedName = RoleNames.EMPLOYEE.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (!dbRoleNames.Any((role) => role == RoleNames.MANAGER))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.MANAGER,
                    NormalizedName = RoleNames.MANAGER.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (!dbRoleNames.Any((role) => role == RoleNames.ADMIN))
            {
                _userContext.Add(new IdentityRole
                {
                    Name = RoleNames.ADMIN,
                    NormalizedName = RoleNames.ADMIN.ToUpperInvariant()
                });
                anyAddition = true;
            }
            if (anyAddition)
            {
                _userContext.SaveChanges();
            }
        }
    }
}
