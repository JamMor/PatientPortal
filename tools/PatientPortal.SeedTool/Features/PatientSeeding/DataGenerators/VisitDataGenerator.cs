using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;

/// <summary>
/// Generates fake Visit data.
/// </summary>
public class VisitDataGenerator
{
    /// <summary>
    /// Generates fake Visit objects without navigation properties, suitable for embedding within related entities.
    /// </summary>
    /// <param name="count">Number of visits to generate</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible visit date</param>
    /// <returns>List of generated Visit objects</returns>
    public List<Visit> GenerateVisitsWithoutNavProps(
        int count,
        List<int> staffIds,
        DateTime earliestDate
    ) => CreateFaker(null, staffIds, earliestDate).Generate(count);

    /// <summary>
    /// Generates fake Visit objects with a specified PatientId, suitable for direct saving with established relationships.
    /// </summary>
    /// <param name="count">Number of visits to generate</param>
    /// <param name="patientId">ID of the patient</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible visit date</param>
    /// <returns>List of generated Visit objects</returns>
    public List<Visit> GenerateVisitsWithPatientId(
        int count,
        int patientId,
        List<int> staffIds,
        DateTime earliestDate
    ) => CreateFaker(patientId, staffIds, earliestDate).Generate(count);

    private static Faker<Visit> CreateFaker(
        int? patientId,
        List<int> staffIds,
        DateTime earliestDate
    )
    {
        var visitFaker = new Faker<Visit>();

        if (patientId.HasValue)
            visitFaker.RuleFor(v => v.PatientId, patientId.Value);

        return visitFaker
            .RuleFor(v => v.StaffId, f => f.PickRandom(staffIds))
            .RuleFor(v => v.Comment, f => f.Random.ClampString(f.Lorem.Paragraph(2), 5))
            .RuleFor(
                v => v.DateOfVisit,
                f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5))
            )
            .RuleFor(v => v.CreatedAt, (f, v) => v.DateOfVisit)
            .RuleFor(v => v.UpdatedAt, (f, v) => v.DateOfVisit);
    }
}
