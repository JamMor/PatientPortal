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
    /// Generates fake TestResult objects without navigation properties, suitable for embedding within related entities.
    /// </summary>
    /// <param name="count">Number of test results to generate</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible test date</param>
    /// <returns>List of generated TestResult objects</returns>
    public List<TestResult> GenerateTestResultsWithoutNavProps(int count, List<int> staffIds, DateTime earliestDate)
        => CreateFaker(null, staffIds, earliestDate).Generate(count);

    /// <summary>
    /// Generates fake TestResult objects with a specified PatientId, suitable for direct saving with established relationships.
    /// </summary>
    /// <param name="count">Number of test results to generate</param>
    /// <param name="patientId">ID of the patient</param>
    /// <param name="staffIds">List of Staff IDs</param>
    /// <param name="earliestDate">Earliest possible test date</param>
    /// <returns>List of generated TestResult objects</returns>
    public List<TestResult> GenerateTestResultsWithPatientId(int count, int patientId, List<int> staffIds, DateTime earliestDate)
        => CreateFaker(patientId, staffIds, earliestDate).Generate(count);

    private static Faker<TestResult> CreateFaker(int? patientId, List<int> staffIds, DateTime earliestDate)
    {
        var testResultFaker = new Faker<TestResult>();

        if (patientId.HasValue)
            testResultFaker.RuleFor(t => t.PatientId, patientId.Value);

        return testResultFaker
            .RuleFor(t => t.StaffId, f => f.PickRandom(staffIds))
            .RuleFor(t => t.Type, f => f.PickRandom(TestTypes))
            .RuleFor(t => t.Comment, f => f.Lorem.Paragraph(2))
            .RuleFor(t => t.CreatedAt, f => f.Date.Between(earliestDate, DateTime.Today.AddDays(-5)))
            .RuleFor(t => t.UpdatedAt, (f, t) => t.CreatedAt);
    }
}
