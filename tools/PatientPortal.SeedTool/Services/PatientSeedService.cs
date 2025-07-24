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
    private readonly PatientDataGenerator _patientDataGenerator;
    private readonly ILogger<PatientSeedService> _logger;

    public PatientSeedService(
        PatientPortalContext context,
        PatientDataGenerator patientDataGenerator,
        ILogger<PatientSeedService> logger)
    {
        _context = context;
        _patientDataGenerator = patientDataGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Seeds a specified number of fake patients.
    /// </summary>
    /// <param name="patientCount">Number of patients to create</param>
    /// <returns>Number of successfully created patients</returns>
    public async Task<int> SeedPatientsAsync(int patientCount)
    {
        if (patientCount <= 0) return 0;
        _logger.LogInformation("Generating {Count} patients...", patientCount);

        var patientList = _patientDataGenerator.GenerateNPatients(patientCount);

        _context.Patients.AddRange(patientList);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Patient seeding complete: {Count} created", patientCount);

        return patientCount;
    }
}
