using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MiniBlog.Models;

namespace MiniBlog.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminEmail = "admin@miniblog.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
                return; // ✅ Admin already exists, do nothing

            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create admin user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
