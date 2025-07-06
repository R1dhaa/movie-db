using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using OfficeOpenXml;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RazorPagesMovie
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            builder.Services.AddRazorPages();
                 var movieDbConnection = builder.Configuration.GetConnectionString("RazorPagesMovieContext");
            if (string.IsNullOrEmpty(movieDbConnection) || movieDbConnection.Contains("Server="))
            {
                // fallback to SQLite file
                var dbPath = Path.Combine(builder.Environment.ContentRootPath, "movies.db");
                movieDbConnection = $"Data Source={dbPath}";
            }

            builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
                options.UseSqlite(movieDbConnection));

            var identityDbConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(identityDbConnection) || identityDbConnection.Contains("Server="))
            {
                var identityDbPath = Path.Combine(builder.Environment.ContentRootPath, "identity.db");
                identityDbConnection = $"Data Source={identityDbPath}";
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(identityDbConnection));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                 options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Optional timeout
    options.SlidingExpiration = false;                // Optional
    options.Cookie.IsEssential = true;                // GDPR compliance
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "RazorPagesMovieAuth";

    // 👇 This makes it non-persistent
    options.LoginPath = "/Login"; 
    options.LogoutPath = "/Logout";
    options.Cookie.MaxAge = null;
            });
            // Add authentication & authorization
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<RazorPagesMovieContext>();
                context.Database.Migrate();

                SeedData.Initialize(services);
                await RoleInitializer.SeedRoles(services);       // seeds roles
                await RoleInitializer.SeedAdminUser(services);   // seeds admin user


            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
        {
            if (context.User.Identity.IsAuthenticated)
            {
                await context.SignOutAsync();
            }
            await next();
        });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/ChooseRole");
                await Task.CompletedTask;
            });

            app.MapRazorPages();
            await app.RunAsync();
        }
    }
}     