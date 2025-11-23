using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DMS.Infrastructure.Data.Config
{
    public class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            // FIRST: Create roles
            await SeedUserRolesAsync(roleManager);

            // THEN: Create regular user
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    DisplayName = "Geralt",
                    Email = "Geralt@Rivia.com",
                    UserName = "Geralt@Rivia.com",
                    Workspace = new Workspace
                    {
                        Name = "GeraltWorkspace",
                    }
                };
                var result = await userManager.CreateAsync(user, password: "`£!t78j-YT5K");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    // Add Member role to regular user
                    await userManager.AddToRoleAsync(user, "Member");
                }
            }

            // Create admin user
            string email = "admin@admin.com";
            string password = "P@$$w0rd1";

            var existingAdmin = await userManager.FindByEmailAsync(email);
            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    DisplayName = "Admin",
                    Email = email,
                    UserName = email,
                    Workspace = new Workspace
                    {
                        Name = "AdminWorkspace",
                    }
                };

                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("✔ Admin user created successfully with Admin role");
                }
                else
                {
                    Console.WriteLine("❌ Failed to create admin user:");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"  - {error.Description}");
                    }
                }
            }
            else
            {
                // Ensure existing admin has the Admin role
                var roles = await userManager.GetRolesAsync(existingAdmin);
                if (!roles.Contains("Admin"))
                {
                    await userManager.AddToRoleAsync(existingAdmin, "Admin");
                    Console.WriteLine("✔ Added Admin role to existing admin user");
                }
            }
        }

        public static async Task SeedUserRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new[] { "Admin", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
                    Console.WriteLine($"✔ Created role: {role}");
                }
            }
        }
    }
}