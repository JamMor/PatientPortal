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
    private readonly PatientDataComposer _patientDataComposer;

    public PatientSeedService(
        PatientPortalContext context,
        ILogger<PatientSeedService> logger,
        PatientDataComposer patientDataComposer)
    {
        _context = context;
        _logger = logger;
        _patientDataComposer = patientDataComposer;
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

        List<int> currentStaffIds = await _context.Staff
            .Select(s => s.StaffId)
            .ToListAsync();

        var patientList = _patientDataComposer.CreatePatients(patientCount);

        // Phase 1: Compose patients with independent data
        foreach (var patient in patientList)
        {
            _patientDataComposer.AttachIndependentData(patient, currentStaffIds);
        }

        _context.Patients.AddRange(patientList);
        await _context.SaveChangesAsync();

        // TODO: Create DTOs so as not to rely on assumed Patient properties population
        // Phase 2: Create health issues with associated visits and tests (requires PatientId)
        var relatedHealthIssues = _patientDataComposer.CreateHealthIssuesWithVisitsAndTests(patientList);
        _context.HealthIssues.AddRange(relatedHealthIssues);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} patients", patientCount);

        return patientCount;
    }
}
