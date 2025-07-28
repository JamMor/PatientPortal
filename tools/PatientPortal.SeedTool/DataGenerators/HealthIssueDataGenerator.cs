using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake HealthIssue data.
/// </summary>
public class HealthIssueDataGenerator
{
    /// <summary>
    /// Generates a specified number of standalone health issues for a patient.
    /// </summary>
    /// <param name="count">Number of health issues to generate</param>
    /// <param name="earliestDate">Earliest possible issue date</param>
    /// <returns>List of generated HealthIssue objects</returns>
    public List<HealthIssue> GenerateNStandaloneHealthIssuesForPatient(int count, DateTime earliestDate)
    {
        var faker = new Faker<HealthIssue>()
            .RuleFor(h => h.ShortDescription, f => f.Random.ClampString(f.Lorem.Sentence(2, 5), 5, 20))
            .RuleFor(h => h.LongDescription, f => f.Lorem.Paragraph(2))
            .RuleFor(h => h.CreatedAt, f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5)))
            .RuleFor(h => h.UpdatedAt, (f, h) => h.CreatedAt);

        return faker.Generate(count);
    }
}
