using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientPortal.SeedTool;
using PatientPortal.SeedTool.Configuration;
using PatientPortal.SeedTool.Validation;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true).Build();

Option<int> staffOption = new("--staff")
{
    Description = "Number of staff members to seed",
    Validators = { CliOptionsValidators.IsPositiveInteger() },
};
Option<int> patientsOption = new("--patients")
{
    Description = "Number of patients to seed",
    Validators = { CliOptionsValidators.IsPositiveInteger() },
};
Option<bool> messagesOption = new("--messages")
{
    Description = "Seed conversations and messages for all messaging links under the conversation threshold",
    DefaultValueFactory = parseresult => false,
};
Option<bool> presetOption = new("--presets")
{
    Description = "Seed preset staff members and patients",
    DefaultValueFactory = parseresult => false,
};
Option<string> connectionStringOption = new("--connection-string")
{
    Description = "Database connection string (optional, reads from main project's appsettings.json if not provided)",
    DefaultValueFactory = parseresult => config["DBInfo:ConnectionString"] ?? string.Empty,
    Validators = { CliOptionsValidators.DoesConnectionStringExist() },
};

var rootCommand = new RootCommand("PatientPortal Database Seeding Tool - Seeds fake staff and patient data")
{
    staffOption,
    patientsOption,
    messagesOption,
    presetOption,
    connectionStringOption,
};

rootCommand.SetAction(async parseresult =>
{
    int staffToGenerate = parseresult.GetValue(staffOption);
    int patientsToGenerate = parseresult.GetValue(patientsOption);
    bool seedMessages = parseresult.GetValue(messagesOption);
    bool seedPresets = parseresult.GetValue(presetOption);
    //TODO: Why does this string need to be nullable? The default value factory
    // should ensure it's never null, but the compiler isn't convinced.
    string? connectionString = parseresult.GetValue(connectionStringOption);
    if (staffToGenerate == 0 && patientsToGenerate == 0 && !seedMessages && !seedPresets)
    {
        Console.WriteLine("No operations specified. Exiting.");
        return 1;
    }
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("No connection string provided. Cannot seed database.");
        return 1;
    }

    try
    {
        // Build service provider with all required services
        using var serviceProvider = ServiceConfiguration.BuildServiceProvider(connectionString);

        // Create scope for scoped services (DbContext, etc.)
        using var scope = serviceProvider.CreateScope();

        // Resolve orchestrator from DI container
        var orchestrator = scope.ServiceProvider.GetRequiredService<SeedOrchestrator>();

        // Execute seeding
        await orchestrator.SeedDatabaseAsync(
            staffToGenerate,
            patientsToGenerate,
            seedMessages,
            seedPresets
        );

        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"✗ Fatal error: {ex.Message}");
        return 1;
    }
});

return await rootCommand.Parse(args).InvokeAsync();
