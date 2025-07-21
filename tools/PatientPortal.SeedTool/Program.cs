using System.CommandLine;
using System.CommandLine.Parsing;

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
Option<string?> connectionStringOption = new("--connection-string")
{
    Description = "Database connection string (optional, reads from environment if not provided)"
};

var rootCommand = new RootCommand("PatientPortal Database Seeding Tool - Seeds fake staff and patient data")
{
    staffOption,
    patientsOption,
    connectionStringOption
};

rootCommand.SetAction(parseresult =>
{
    int staffToGenerate = parseresult.GetValue(staffOption);
    int patientsToGenerate = parseresult.GetValue(patientsOption);
    string? connectionString = parseresult.GetValue(connectionStringOption);
    ConsoleLogThem(staffToGenerate, patientsToGenerate, connectionString);
    if (staffToGenerate == 0 && patientsToGenerate == 0)
    {
        Console.WriteLine("No staff or patients to generate. Exiting.");
        return 1;
    }
    else
    {
        Console.WriteLine($"Would seed the database with {staffToGenerate} staff members and {patientsToGenerate} patients here.");
    }
    return 0;
});

return rootCommand.Parse(args).Invoke();

static void ConsoleLogThem(int staff, int patients, string? connectionString)
{
    Console.WriteLine("=== PatientPortal Seed Tool ===");
    Console.WriteLine($"Staff to create: {staff}");
    Console.WriteLine($"Patients to create: {patients}");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("No connection string provided.");
    }
    else
    {
        Console.WriteLine($"Connection string provided: {connectionString}");
    }
    Console.WriteLine();
    Console.WriteLine("================================================");
}

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