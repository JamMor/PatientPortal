using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.SeedTool.Services;

namespace PatientPortal.SeedTool;

/// <summary>
/// Orchestrates the database seeding process.
/// </summary>
public class SeedOrchestrator
{
    private readonly PatientPortalContext _context;
    private readonly StaffSeedService _staffSeedService;
    private readonly PatientSeedService _patientSeedService;
    private readonly ILogger<SeedOrchestrator> _logger;

    public SeedOrchestrator(
        PatientPortalContext context, 
        StaffSeedService staffSeedService,
        PatientSeedService patientSeedService,
        ILogger<SeedOrchestrator> logger)
    {
        _context = context;
        _staffSeedService = staffSeedService;
        _patientSeedService = patientSeedService;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with the specified number of staff and patients.
    /// </summary>
    public async Task SeedDatabaseAsync(int staffCount, int patientCount)
    {
        ConsoleWrites.WriteHeader();
        ConsoleWrites.WriteOperationParams(staffCount, patientCount);

        try
        {
            // Get current counts
            int currentStaff = await _context.Staff.CountAsync();
            int currentPatients = await _context.Patients.CountAsync();
            
            _logger.LogInformation("Database connected - Current: {Staff} staff, {Patients} patients", 
                currentStaff, currentPatients);

            // Seed staff
            if (staffCount > 0)
            {
                await _staffSeedService.SeedStaffAsync(staffCount);
            }
            
            // Seed patients
            if (patientCount > 0)
            {
                await _patientSeedService.SeedPatientsAsync(patientCount);
            }

            // Display final summary
            int finalStaff = await _context.Staff.CountAsync();
            int finalPatients = await _context.Patients.CountAsync();
            
            ConsoleWrites.WriteOperationResults(currentStaff, currentPatients, finalStaff, finalPatients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seeding failed");
            throw;
        }
    }
}