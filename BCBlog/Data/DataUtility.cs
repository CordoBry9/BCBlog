using BCBlog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public static class DataUtility
{
    private const string _adminRole = "Author";
    private const string _moderatorRole = "Moderator";

    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");  // Local Connection string
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PRIVATE_URL");  // Remote connection string

        return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);
    }

    private static string BuildConnectionString(string databaseUrl)
    {
        //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/'),
            SslMode = SslMode.Prefer
        };
        return builder.ToString();
    }

    public static async Task ManageDataAsync(IServiceProvider svcProvider)
    {
        //Service: An instance of RoleManager
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        //Service: An instance of Configuration Service
        var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();
        //Service: An instance of the UserManager
        var userManagerSvc = svcProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //Service: An instance of RoleManager
        var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

        //Migration: This is the programmatic equivalent to Update-Database
        await dbContextSvc.Database.MigrateAsync();


        // Seed Roles
        await SeedRolesAsync(roleManagerSvc);

        //Seed Users
        await SeedUsersAsync(dbContextSvc, configurationSvc, userManagerSvc);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(_adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(_adminRole));
        }
        if (!await roleManager.RoleExistsAsync(_moderatorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(_moderatorRole));
        }
    }


    private static async Task SeedUsersAsync(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        // Seed Admin User
        var adminEmail = configuration["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail")!;
        if (!context.Users.Any(u => u.Email == adminEmail))
        {
            ApplicationUser adminUser = new()
            {
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = "Bryan",
                LastName = "Cordova",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, configuration["AdminPassword"] ?? Environment.GetEnvironmentVariable("AdminPassword")!);
            await userManager.AddToRoleAsync(adminUser, _adminRole);
        }

        // Seed Moderator User
        var moderatorEmail = configuration["ModeratorEmail"] ?? Environment.GetEnvironmentVariable("ModeratorEmail")!;
        if (!context.Users.Any(u => u.Email == moderatorEmail))
        {
            ApplicationUser moderatorUser = new()
            {
                Email = moderatorEmail,
                UserName = moderatorEmail,
                FirstName = "Bryan",
                LastName = "Cordova",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(moderatorUser, configuration["ModeratorPassword"] ?? Environment.GetEnvironmentVariable("ModeratorPassword")!);
            await userManager.AddToRoleAsync(moderatorUser, _moderatorRole);
        }
    }
}
