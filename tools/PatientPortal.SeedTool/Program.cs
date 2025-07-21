using System.CommandLine;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Configuration;
using PatientPortal.SeedTool;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

Option<int> staffOption = new("--staff")
{
    Description = "Number of staff members to seed",
    Validators = { IsPositiveInteger() }
};
Option<int> patientsOption = new("--patients")
{
    Description = "Number of patients to seed",
    Validators = { IsPositiveInteger() }
};
Option<string> connectionStringOption = new("--connection-string")
{
    Description = "Database connection string (optional, reads from main project's appsettings.json if not provided)",
    DefaultValueFactory = parseresult => config["DBInfo:ConnectionString"] ?? string.Empty,
    Validators = { DoesConnectionStringExist() }
};

var rootCommand = new RootCommand("PatientPortal Database Seeding Tool - Seeds fake staff and patient data")
{
    staffOption,
    patientsOption,
    connectionStringOption
};

rootCommand.SetAction(async parseresult =>
{
    int staffToGenerate = parseresult.GetValue(staffOption);
    int patientsToGenerate = parseresult.GetValue(patientsOption);
    //TODO: Why does this string need to be nullable? The default value factory 
    // should ensure it's never null, but the compiler isn't convinced.
    string? connectionString = parseresult.GetValue(connectionStringOption);
    if (staffToGenerate == 0 && patientsToGenerate == 0)
    {
        Console.WriteLine("No staff or patients to generate. Exiting.");
        return 1;
    }
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("No connection string provided. Cannot seed database.");
        return 1;
    }

    await Seeder.SeedDatabase(staffToGenerate, patientsToGenerate, connectionString);
    return 0;
});

return await rootCommand.Parse(args).InvokeAsync();

static Action<OptionResult> IsPositiveInteger()
{
    return result =>
    {
        if (result.GetValueOrDefault<int>() < 0)
        {
            result.AddError("Patients must be a non-negative integer.");
        }
    };
}
static Action<OptionResult> DoesConnectionStringExist()
{
    return result =>
    {
        if (string.IsNullOrEmpty(result.GetValueOrDefault<string>()))
        {
            result.AddError("Connection string must be provided in command line, or in appsettings.json.");
        }
    };
}