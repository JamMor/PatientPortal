using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;

/// <summary>
/// Generates fake HealthIssue data.
/// </summary>
public class HealthIssueDataGenerator
{
    /// <summary>
    /// Generates fake HealthIssue objects without navigation properties, suitable for embedding within related entities.
    /// </summary>
    /// <param name="count">Number of health issues to generate</param>
    /// <param name="earliestDate">Earliest possible issue date</param>
    /// <returns>List of generated HealthIssue objects</returns>
    public List<HealthIssue> GenerateHealthIssuesWithoutNavProps(
        int count,
        DateTime earliestDate
    ) => CreateFaker(null, earliestDate, null, null).Generate(count);

    /// <summary>
    /// Generates fake HealthIssue objects with a specified PatientId, suitable for direct saving with established relationships.
    /// </summary>
    /// <param name="count">Number of health issues to generate</param>
    /// <param name="patientId">ID of the patient</param>
    /// <param name="earliestDate">Earliest possible issue date</param>
    /// <returns>List of generated HealthIssue objects</returns>
    public List<HealthIssue> GenerateHealthIssuesWithPatientId(
        int count,
        int patientId,
        DateTime earliestDate
    ) => CreateFaker(patientId, earliestDate, null, null).Generate(count);

    /// <summary>
    /// Generates a single <see cref="HealthIssue"/> with preset descriptions.
    /// The date is Faker-generated between <paramref name="earliestDate"/> and five days ago.
    /// </summary>
    public HealthIssue GenerateFromDescriptions(
        int patientId,
        DateTime earliestDate,
        string shortDescription,
        string longDescription
    ) => CreateFaker(patientId, earliestDate, shortDescription, longDescription).Generate();

    private static Faker<HealthIssue> CreateFaker(
        int? patientId,
        DateTime earliestDate,
        string? shortDesc,
        string? longDesc = null
    )
    {
        var healthIssueFaker = new Faker<HealthIssue>();

        if (patientId.HasValue)
            healthIssueFaker.RuleFor(h => h.PatientId, patientId.Value);

        return healthIssueFaker
            .RuleFor(
                h => h.ShortDescription,
                f => shortDesc ?? f.Random.ClampString(f.Lorem.Sentence(2, 5), 5, 20)
            )
            .RuleFor(h => h.LongDescription, f => longDesc ?? f.Lorem.Paragraph(2))
            .RuleFor(
                h => h.CreatedAt,
                f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5))
            )
            .RuleFor(h => h.UpdatedAt, (f, h) => h.CreatedAt);
    }
}
