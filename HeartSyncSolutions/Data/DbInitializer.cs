using HeartSyncSolutions.Models;
using Microsoft.AspNetCore.Identity;

namespace HeartSyncSolutions.Data
{
    public static class DbInitializer
    {
        // This method is called from Program.cs
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Define the roles
            string[] roleNames = { "Admin", "User" };

            // Create roles if they don't exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create the admin user
            var adminEmail = "admin@heartsync.com";
            var adminPassword = "Admin123$"; // Change this to a more secure password

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create the admin user
                var newAdmin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    ContactNumber = "0000000000",
                    EmailConfirmed = true,
                    IsVolunteer = false,
                    IsDonor = false
                };

                var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);

                if (createAdminResult.Succeeded)
                {
                    // Assign the admin role to the admin user
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    Console.WriteLine("Admin user created successfully!");
                }
                else
                {
                    Console.WriteLine("Failed to create admin user:");
                    foreach (var error in createAdminResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                    }
                }
            }
            else
            {
                // Ensure existing admin has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}