using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DMS.Services.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<User> FindUserByClaimWithWorkspace(this UserManager<User> userManager, ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            var res = await userManager.Users.Include(x => x.Workspace)
                                          .SingleOrDefaultAsync(x => x.Email == email);
            if (res == null)
            {
                throw new Exception("Something went wrong");
            }
            return res;
        }

        public static async Task<User> FindEmailByClaim(this UserManager<User> userManager, ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            var res = await userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (res == null)
            {
                throw new Exception("Something went wrong");
            }
            return res;
        }
    }
}
