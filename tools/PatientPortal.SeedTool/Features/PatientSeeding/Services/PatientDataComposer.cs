using PatientPortal.Models;
using PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;

namespace PatientPortal.SeedTool.Features.PatientSeeding.Services;

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

    private const int MaxRelatedHealthIssuesPerPatient = 3;
    private const int MaxVisitsPerHealthIssue = 4;
    private const int MaxTestResultsPerHealthIssue = 3;

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

    /// <summary>
    /// Creates health issues with associated visits and test results for a list of patients.
    /// Requires patients to have valid PatientIds and medical team from the database.
    /// </summary>
    /// <param name="patients">List of persisted patients (must have PatientId and medical team)</param>
    /// <returns>List of HealthIssue objects with associated visits and test results</returns>
    public List<HealthIssue> CreateHealthIssuesWithVisitsAndTests(List<Patient> patients)
    {
        var allIssues = new List<HealthIssue>();

        foreach (var patient in patients)
        {
            int issueCount = BetweenOneAnd(MaxRelatedHealthIssuesPerPatient);
            List<HealthIssue> issues = _healthIssueDataGenerator.GenerateHealthIssuesWithPatientId(
                issueCount,
                patient.PatientId,
                patient.CreatedAt);

            foreach (var issue in issues)
            {
                List<int> medicalTeamIds = patient.MedicalTeam.Select(mt => mt.StaffId).ToList();

                List<Visit> visits = _visitDataGenerator.GenerateVisitsWithPatientId(
                    BetweenOneAnd(MaxVisitsPerHealthIssue),
                    patient.PatientId,
                    medicalTeamIds,
                    issue.CreatedAt);

                List<TestResult> tests = _testResultDataGenerator.GenerateTestResultsWithPatientId(
                    BetweenOneAnd(MaxTestResultsPerHealthIssue),
                    patient.PatientId,
                    medicalTeamIds,
                    issue.CreatedAt);

                issue.AssociatedVisits = visits
                    .Select(v => new VisitHealthIssueAssociation { Visit = v })
                    .ToList();

                issue.AssociatedTestResults = tests
                    .Select(t => new TestHealthIssueAssociation { TestResult = t })
                    .ToList();
            }

            allIssues.AddRange(issues);
        }

        return allIssues;
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
