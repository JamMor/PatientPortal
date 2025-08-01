using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;

/// <summary>
/// Generates fake PatientStaffConnection data.
/// </summary>
public class PatientStaffConnectionDataGenerator
{
    /// <summary>
    /// Generates patient-staff connections for the provided staff IDs.
    /// </summary>
    /// <param name="staffIds">List of staff IDs to create connections for</param>
    /// <param name="createdAt">Connection creation date</param>
    /// <returns>List of PatientStaffConnection objects</returns>
    public List<PatientStaffConnection> GenerateConnectionsForStaffIds(List<int> staffIds, DateTime createdAt)
    {
        var patientStaffConnectionFaker = new Faker<PatientStaffConnection>()
            .RuleFor(c => c.StaffId, f => staffIds[f.IndexFaker])
            .RuleFor(c => c.CreatedAt, createdAt)
            .RuleFor(c => c.UpdatedAt, createdAt);

        return patientStaffConnectionFaker.Generate(staffIds.Count);
    }
}
