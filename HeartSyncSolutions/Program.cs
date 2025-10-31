using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models; 
using HeartSyncSolutions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. Add services to the container ---

            // Grab the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Register our DbContext to use SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add Database Developer Page Exception Filter
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Configure Identity to use ApplicationUser (single configuration)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Sign in settings
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI(); // This adds the default Identity UI

            // Configure cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });
            
            // Register custom services
            builder.Services.AddScoped<IDonationService, DonationService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllersWithViews();

            // Add Razor Pages for Identity UI
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // --- 2. Seed roles on startup ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                try
                {
                    await SeedRolesAsync(services, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding roles.");
                }
            }

            // --- 3. Configure the HTTP request pipeline ---

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map controller routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Map Razor Pages for Identity UI
            app.MapRazorPages();

            app.Run();
        }

        // Method to seed default roles
        private static async Task SeedRolesAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                
                if (!roleExist)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"✅ Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"❌ Failed to create role '{roleName}'.");
                    }
                }
                else
                {
                    logger.LogInformation($"ℹ️ Role '{roleName}' already exists.");
                }
            }
        }
    }
}