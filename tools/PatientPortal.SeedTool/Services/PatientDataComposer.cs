using PatientPortal.Models;
using PatientPortal.SeedTool.DataGenerators;

namespace PatientPortal.SeedTool.Services;

/// <summary>
/// Composes patient data by orchestrating multiple data generators.
/// Handles the composition logic for creating fully populated patient entities.
/// </summary>
public class PatientDataComposer
{
    private readonly PatientDataGenerator _patientDataGenerator;
    private readonly AddressDataGenerator _addressDataGenerator;
    private readonly PatientStaffConnectionDataGenerator _connectionDataGenerator;
    private readonly VisitDataGenerator _visitDataGenerator;
    private readonly TestResultDataGenerator _testResultDataGenerator;
    private readonly HealthIssueDataGenerator _healthIssueDataGenerator;

    public PatientDataComposer(
        PatientDataGenerator patientDataGenerator,
        AddressDataGenerator addressDataGenerator,
        PatientStaffConnectionDataGenerator connectionDataGenerator,
        VisitDataGenerator visitDataGenerator,
        TestResultDataGenerator testResultDataGenerator,
        HealthIssueDataGenerator healthIssueDataGenerator)
    {
        _patientDataGenerator = patientDataGenerator;
        _addressDataGenerator = addressDataGenerator;
        _connectionDataGenerator = connectionDataGenerator;
        _visitDataGenerator = visitDataGenerator;
        _testResultDataGenerator = testResultDataGenerator;
        _healthIssueDataGenerator = healthIssueDataGenerator;
    }

    // Configuration constants for data composition
    private const double AddressProbability = 0.33; // 33% chance of having an address
    private const int MaxStaffPerPatient = 3;
    private const int MaxIndependentVisitsPerPatient = 3;
    private const int MaxIndependentTestResultsPerPatient = 2;
    private const int MaxIndependentHealthIssuesPerPatient = 2;

    /// <summary>
    /// Creates a list of patients with basic demographic data.
    /// This does not include any related entities or associations.
    /// </summary>
    /// <param name="count">Number of patients to create</param>
    /// <returns>List of Patient objects with basic data</returns>
    public List<Patient> CreatePatients(int count)
    {
        return _patientDataGenerator.GenerateNPatients(count);
    }

    /// <summary>
    /// Attaches independent data to a patient (address, staff connections, visits, tests, health issues).
    /// These entities are not yet associated with each other.
    /// </summary>
    /// <param name="patient">Patient to attach data to</param>
    /// <param name="availableStaffIds">List of all available staff IDs</param>
    public void AttachIndependentData(Patient patient, List<int> availableStaffIds)
    {
        if (Random.Shared.NextDouble() < AddressProbability)
        {
            patient.Address = _addressDataGenerator.GenerateAddress();
        }

        // Select random staff members for medical team
        List<int> selectedStaffIds = GetRandomSubset(availableStaffIds, BetweenOneAnd(MaxStaffPerPatient));

        patient.MedicalTeam = _connectionDataGenerator.GenerateConnectionsForStaffIds(selectedStaffIds, patient.CreatedAt);

        patient.Visits = _visitDataGenerator.GenerateVisitsWithoutNavProps(
            BetweenOneAnd(MaxIndependentVisitsPerPatient),
            selectedStaffIds,
            patient.CreatedAt);

        patient.Tests = _testResultDataGenerator.GenerateTestResultsWithoutNavProps(
            BetweenOneAnd(MaxIndependentTestResultsPerPatient),
            selectedStaffIds,
            patient.CreatedAt);

        patient.HealthIssues = _healthIssueDataGenerator.GenerateHealthIssuesWithoutNavProps(
            BetweenOneAnd(MaxIndependentHealthIssuesPerPatient),
            patient.CreatedAt);
    }

    private static int BetweenOneAnd(int max)
    {
        return Random.Shared.Next(1, max + 1);
    }

    private static List<T> GetRandomSubset<T>(List<T> list, int count)
    {
        return list.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
    }
}
