using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.Features.StaffSeeding.Services;
using PatientPortal.SeedTool.Features.PatientSeeding.Services;
using PatientPortal.SeedTool.Features.MessageSeeding.Services;
using PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;
using PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;
using PatientPortal.SeedTool.Features.MessageSeeding.DataGenerators;

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
    /// <param name="logLevel">Minimum log level for application logs (default: Information). Use LogLevel.Debug to see SQL queries.</param>
    /// <returns>Configured ServiceProvider</returns>
    public static ServiceProvider BuildServiceProvider(string connectionString, LogLevel logLevel = LogLevel.Information)
    {
        var services = new ServiceCollection();

        // Register logging with filters to suppress verbose EF Core SQL queries
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(logLevel);

            // Suppress EF Core database command logging (SQL queries) unless Debug or Trace level
            if (logLevel > LogLevel.Debug)
            {
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                builder.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
            }
        });

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

        // Register data generators (stateless, can be singleton)
        services.AddSingleton<StaffDataGenerator>();
        services.AddSingleton<IdentityUserDataGenerator>();
        services.AddSingleton<PatientDataGenerator>();
        services.AddSingleton<AddressDataGenerator>();
        services.AddSingleton<PatientStaffConnectionDataGenerator>();
        services.AddSingleton<VisitDataGenerator>();
        services.AddSingleton<TestResultDataGenerator>();
        services.AddSingleton<HealthIssueDataGenerator>();
        services.AddSingleton<ConversationDataGenerator>();
        services.AddSingleton<MessageDataGenerator>();

        // Data composers (stateless, singleton)
        services.AddSingleton<PatientDataComposer>();
        services.AddSingleton<MessagingDataComposer>();

        // Seed services (stateful, scoped to match DbContext)
        services.AddScoped<SeedOrchestrator>();
        services.AddScoped<StaffSeedService>();
        services.AddScoped<PatientSeedService>();
        services.AddScoped<MessagingSeedService>();

        return services.BuildServiceProvider();
    }
}
