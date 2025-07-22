using System.CommandLine.Parsing;

namespace PatientPortal.SeedTool.Validation;

/// <summary>
/// Validators for command-line options.
/// </summary>
public static class CliOptionsValidators
{
    /// <summary>
    /// Validates that an integer option is non-negative.
    /// </summary>
    public static Action<OptionResult> IsPositiveInteger()
    {
        return result =>
        {
            if (result.GetValueOrDefault<int>() < 0)
            {
                result.AddError("Value must be a non-negative integer.");
            }
        };
    }

    /// <summary>
    /// Validates that a connection string is provided (either from command line or configuration).
    /// </summary>
    public static Action<OptionResult> DoesConnectionStringExist()
    {
        return result =>
        {
            if (string.IsNullOrEmpty(result.GetValueOrDefault<string>()))
            {
                result.AddError("Connection string must be provided in command line, or in appsettings.json.");
            }
        };
    }
}
