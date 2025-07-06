using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using OfficeOpenXml;
using Microsoft.AspNetCore.Identity;
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

            // Fix ApplicationDbContext connection string for SQLite
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
                options.LoginPath = "/Login"; // redirects here if not signed in
            });
            // Add authentication & authorization
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
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