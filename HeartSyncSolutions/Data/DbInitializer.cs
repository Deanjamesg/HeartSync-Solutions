using HeartSyncSolutions.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Data
{
    public static class DbInitializer
    {
        // This is the main method we'll call from Program.cs
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Get the services we need to do the work
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Make sure the database is actually created (good for dev)
            context.Database.EnsureCreated();

            // --- 1. Seed Roles ---
            await SeedRolesAsync(roleManager);

            // --- 2. Seed Admin User ---
            await SeedAdminUserAsync(userManager);

            // --- 3. Seed Lookup Tables ---
            await SeedEventTypesAsync(context);
            await SeedEventStatusesAsync(context);
            await SeedAttendanceStatusesAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Check if any roles already exist
            if (await roleManager.Roles.AnyAsync())
            {
                return; // DB has been seeded
            }

            // Create the roles Frankie mentioned 
            string[] roleNames = { "Admin", "Finance", "Communications", "SocialMedia", "Operations" };

            foreach (var roleName in roleNames)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Check if our default admin user exists
            if (await userManager.FindByEmailAsync("admin@heartsync.local") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@heartsync.local",
                    Email = "admin@heartsync.local",
                    FirstName = "Admin",
                    LastName = "User",
                    ContactNumber = "000000000",
                    EmailConfirmed = true // Confirm email so they can log in right away
                };

                // Create the user with a simple password
                var result = await userManager.CreateAsync(adminUser, "P@ssword123");

                if (result.Succeeded)
                {
                    // Add the new user to the "Admin" role
                    // This gives them full permissions
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedEventTypesAsync(ApplicationDbContext context)
        {
            // Check if event types are already there
            if (await context.EventTypes.AnyAsync())
            {
                return; // DB has been seeded
            }

            var eventTypes = new List<EventType>
            {
                // Based on Frankie's feedback 
                new EventType { Title = "Food Drive" },
                new EventType { Title = "Jacket Drive" },
                new EventType { Title = "Children's Home Visit" },
                new EventType { Title = "Old-Age Home Visit" },
                new EventType { Title = "Prayer Event" }
            };

            await context.EventTypes.AddRangeAsync(eventTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEventStatusesAsync(ApplicationDbContext context)
        {
            if (await context.EventStatuses.AnyAsync())
            {
                return; // DB has been seeded
            }

            var statuses = new List<EventStatus>
            {
                new EventStatus { Status = "Up-coming" }, // Default for new events
                new EventStatus { Status = "Completed" },
                new EventStatus { Status = "Cancelled" }
            };

            await context.EventStatuses.AddRangeAsync(statuses);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAttendanceStatusesAsync(ApplicationDbContext context)
        {
            if (await context.AttendanceStatuses.AnyAsync())
            {
                return; // DB has been seeded
            }

            var statuses = new List<AttendanceStatus>
            {
                new AttendanceStatus { Status = "Signed Up" }, // Default for UC-102
                new AttendanceStatus { Status = "Attended" }, // For admin to check off (UC-202)
                new AttendanceStatus { Status = "No Show" }  // For admin to check off
            };

            await context.AttendanceStatuses.AddRangeAsync(statuses);
            await context.SaveChangesAsync();
        }
    }
}