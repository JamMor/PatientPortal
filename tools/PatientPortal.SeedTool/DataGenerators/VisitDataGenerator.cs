using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake Visit data.
/// </summary>
public class VisitDataGenerator
{
    /// <summary>
    /// Generates a specified number of standalone visits for a patient.
    /// </summary>
    /// <param name="count">Number of visits to generate</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible visit date</param>
    /// <returns>List of generated Visit objects</returns>
    public List<Visit> GenerateNStandaloneVisitsForPatient(int count, List<int> staffIds, DateTime earliestDate)
    {
        var faker = new Faker<Visit>()
            .RuleFor(v => v.StaffId, f => f.PickRandom(staffIds))
            .RuleFor(v => v.Comment, f => f.Lorem.Paragraph(2))
            .RuleFor(v => v.DateOfVisit, f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5)))
            .RuleFor(v => v.CreatedAt, (f, v) => v.DateOfVisit)
            .RuleFor(v => v.UpdatedAt, (f, v) => v.DateOfVisit);

        return faker.Generate(count);
    }
}
