using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.DataGenerators;

namespace PatientPortal.SeedTool.Services;

/// <summary>
/// Service for seeding fake patient data into the database.
/// </summary>
public class PatientSeedService
{
    private readonly PatientPortalContext _context;
    private readonly ILogger<PatientSeedService> _logger;

    // Data generators
    private readonly PatientDataGenerator _patientDataGenerator;
    private readonly AddressDataGenerator _addressDataGenerator;
    private readonly PatientStaffConnectionDataGenerator _connectionDataGenerator;
    private readonly VisitDataGenerator _visitDataGenerator;
    private readonly TestResultDataGenerator _testResultDataGenerator;
    private readonly HealthIssueDataGenerator _healthIssueDataGenerator;

    // Patient related data settings
    private const int MaxStaffPerPatient = 3;
    private const int MaxStandaloneVisitsPerPatient = 3;
    private const int MaxStandaloneTestResultsPerPatient = 2;
    private const int MaxStandaloneHealthIssuesPerPatient = 2;
    private const double AddressProbability = 0.33; // 33% chance of having an address

    public PatientSeedService(
        PatientPortalContext context,
        ILogger<PatientSeedService> logger,
        PatientDataGenerator patientDataGenerator,
        AddressDataGenerator addressDataGenerator,
        PatientStaffConnectionDataGenerator connectionDataGenerator,
        VisitDataGenerator visitDataGenerator,
        TestResultDataGenerator testResultDataGenerator,
        HealthIssueDataGenerator healthIssueDataGenerator)
    {
        _context = context;
        _logger = logger;
        _patientDataGenerator = patientDataGenerator;
        _addressDataGenerator = addressDataGenerator;
        _connectionDataGenerator = connectionDataGenerator;
        _visitDataGenerator = visitDataGenerator;
        _testResultDataGenerator = testResultDataGenerator;
        _healthIssueDataGenerator = healthIssueDataGenerator;
    }

    /// <summary>
    /// Seeds a specified number of fake patients with full medical history.
    /// </summary>
    /// <param name="patientCount">Number of patients to create</param>
    /// <returns>Number of successfully created patients</returns>
    public async Task<int> SeedPatientsAsync(int patientCount)
    {
        if (patientCount <= 0) return 0;

        _logger.LogInformation("Generating {Count} patients with medical data...", patientCount);

        // Get all current staff IDs
        var currentStaffIds = await _context.Staff
            .Select(s => s.StaffId)
            .ToListAsync();

        // Generate basic patients
        var patientList = _patientDataGenerator.GenerateNPatients(patientCount);
        
        // Add related data
        foreach (var patient in patientList)
        {
            // 33% chance of having an address
            if (Random.Shared.NextDouble() < AddressProbability)
            {
                patient.Address = _addressDataGenerator.GenerateAddress();
            }

            // Select random staff members for medical team
            int staffCount = Random.Shared.Next(1, MaxStaffPerPatient + 1);
            var selectedStaffIds = GetRandomSubset(currentStaffIds, staffCount);
            patient.MedicalTeam = _connectionDataGenerator.GenerateConnectionsForStaffIds(selectedStaffIds, patient.CreatedAt);

            // STANDALONE VISITS, TESTS, HEALTH ISSUES

            // Generate standalone visits
            int visitCount = Random.Shared.Next(1, MaxStandaloneVisitsPerPatient + 1);
            patient.Visits = _visitDataGenerator.GenerateNStandaloneVisitsForPatient(
                visitCount, 
                selectedStaffIds, 
                patient.CreatedAt);

            // Generate standalone test results
            int testCount = Random.Shared.Next(1, MaxStandaloneTestResultsPerPatient + 1);
            patient.Tests = _testResultDataGenerator.GenerateNStandaloneTestResultsForPatient(
                testCount, 
                selectedStaffIds, 
                patient.CreatedAt);

            // Generate standalone health issues
            int healthIssueCount = Random.Shared.Next(0, MaxStandaloneHealthIssuesPerPatient + 1);
            if (healthIssueCount > 0)
            {
                patient.HealthIssues = _healthIssueDataGenerator.GenerateNStandaloneHealthIssuesForPatient(
                    healthIssueCount, 
                    patient.CreatedAt);
            }
        }
        
        // Save entire object graph in one transaction
        _context.Patients.AddRange(patientList);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} patients", patientCount);

        return patientCount;
    }

    public List<T>GetRandomSubset<T>(List<T> list, int count)
    {
        return list.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
    }
}
