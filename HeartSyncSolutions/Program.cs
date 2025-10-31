using HeartSyncSolutions.Data;
using HeartSyncSolutions.Models; 
using HeartSyncSolutions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. Add services to the container ---

            // Grab the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Register our DbContext to use SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // This is the main identity setup.
            // We're swapping AddDefaultIdentity for AddIdentity.
            // This lets us use our ApplicationUser and add support for Roles.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Keeping password rules simple for dev
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); // Adds support for password resets

            // Register our custom services for dependency injection
            builder.Services.AddScoped<IDonationService, DonationService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllersWithViews();

            // We need to add Razor Pages because the Identity UI (login/register)
            // is built with Razor Pages, even in an MVC app.
            builder.Services.AddRazorPages();


            var app = builder.Build();

            // --- 2. Configure the HTTP request pipeline ---

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

            // We must add Authentication() *before* Authorization().
            // This finds out who the user is.
            app.UseAuthentication();
            // This checks what they are allowed to do.
            app.UseAuthorization();


            // This is our main route for controllers
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // This maps the routes for the Identity Razor Pages (login/register)
            app.MapRazorPages();

            app.Run();
        }
    }
}