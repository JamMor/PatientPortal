using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Configuration;

/// <summary>
/// Configures dependency injection for the seed tool.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Builds and configures the service provider with all required services.
    /// </summary>
    /// <param name="connectionString">Database connection string for MySQL</param>
    /// <returns>Configured ServiceProvider</returns>
    public static ServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();

        // Register DbContext with MySQL
        services.AddDbContext<PatientPortalContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
        );

        // Register Identity Core (lightweight - only UserManager, no SignInManager/cookies/roles)
        // Configuration must match main app to ensure seeded users are valid
        services.AddIdentityCore<IdentityUser>(options =>
        {
            // Password settings - must match main app for consistent user creation
            options.Password.RequiredLength = 10;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            
            // User settings - must match main app for consistent username validation
            options.User.RequireUniqueEmail = false;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+<>$";
        })
        .AddEntityFrameworkStores<PatientPortalContext>();

        // Register seed services
        services.AddScoped<SeedOrchestrator>();
        // services.AddScoped<StaffSeedService>(); // TODO: Implement StaffSeedService
        // services.AddScoped<PatientSeedService>(); // TODO: Implement PatientSeedService

        return services.BuildServiceProvider();
    }
}
