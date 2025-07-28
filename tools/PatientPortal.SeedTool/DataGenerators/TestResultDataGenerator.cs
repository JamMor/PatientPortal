using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake TestResult data.
/// </summary>
public class TestResultDataGenerator
{
    private static readonly string[] TestTypes = { "Vitals", "Pathology", "Imaging", "Labwork" };

    /// <summary>
    /// Generates a specified number of standalone test results for a patient.
    /// </summary>
    /// <param name="count">Number of test results to generate</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible test date</param>
    /// <returns>List of generated TestResult objects</returns>
    public List<TestResult> GenerateNStandaloneTestResultsForPatient(int count, List<int> staffIds, DateTime earliestDate)
    {
        var faker = new Faker<TestResult>()
            .RuleFor(t => t.StaffId, f => f.PickRandom(staffIds))
            .RuleFor(t => t.Type, f => f.PickRandom(TestTypes))
            .RuleFor(t => t.Comment, f => f.Lorem.Paragraph(2))
            .RuleFor(t => t.CreatedAt, f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5)))
            .RuleFor(t => t.UpdatedAt, (f, t) => t.CreatedAt);

        return faker.Generate(count);
    }
}
