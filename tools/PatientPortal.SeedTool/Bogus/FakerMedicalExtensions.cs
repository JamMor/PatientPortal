using Bogus;
using Bogus.Premium;

namespace PatientPortal.SeedTool.Bogus;

/// <summary>
/// Extension methods that expose custom medical datasets on <see cref="Faker"/>.
/// </summary>
public static class FakerMedicalExtensions
{
    /// <summary>
    /// Provides access to the <see cref="MedicalData"/> dataset for generating realistic medical data.
    /// </summary>
    public static MedicalData Medical(this Faker f)
    {
        return ContextHelper.GetOrSet(f, () => new MedicalData());
    }
}

/// <summary>
/// Custom Bogus dataset providing realistic medical data for seed generation.
/// </summary>
public class MedicalData : DataSet
{
    /// <summary>
    /// Returns a randomly selected plausible health issue short description.
    /// </summary>
    public string HealthIssueShortDescription() => HealthIssueData.GetARandomShortDescription();

    /// <summary>
    /// Returns a randomly selected plausible medical test type name.
    /// </summary>
    public string TestType() => TestTypeData.GetARandomTestType();
}
