using Application.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class DataInitializerService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public async Task InitializeAsync()
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }

            var adminUserName = "admin";
            var adminNormalizedUserName = adminUserName.Normalize().ToUpperInvariant();
            var adminEmail = "admin@example.com";
            var adminNormalizedEmail = adminEmail.Normalize().ToUpperInvariant();
            var adminPassword = "admin";

            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new()
                {
                    UserName = adminUserName,
                    NormalizedUserName = adminNormalizedUserName,
                    Email = adminEmail,
                    NormalizedEmail = adminNormalizedEmail
                };

                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, adminPassword);

                var result = await _userManager.CreateAsync(adminUser);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "admin");
                }
            }
        }
    }
}
