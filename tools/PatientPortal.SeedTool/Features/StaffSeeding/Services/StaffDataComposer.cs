using PatientPortal.Models;
using PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;

namespace PatientPortal.SeedTool.Features.StaffSeeding.Services;

/// <summary>
/// Composes staff data for seeding.
/// </summary>
public class StaffDataComposer(StaffDataGenerator staffDataGenerator)
{
    private readonly StaffDataGenerator _staffDataGenerator = staffDataGenerator;

    /// <summary>
    /// Generates a list of fake staff members.
    /// </summary>
    /// <param name="count">Number of staff members to create</param>
    /// <returns>List of generated Staff objects</returns>
    public List<Staff> CreateStaff(int count)
    {
        return _staffDataGenerator.GenerateNStaff(count);
    }
}
