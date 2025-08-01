using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.SeedTool.Features.StaffSeeding.Services;
using PatientPortal.SeedTool.Features.PatientSeeding.Services;
using PatientPortal.SeedTool.Features.MessageSeeding.Services;

namespace PatientPortal.SeedTool;

/// <summary>
/// Orchestrates the database seeding process.
/// </summary>
public class SeedOrchestrator(
    PatientPortalContext context,
    StaffSeedService staffSeedService,
    PatientSeedService patientSeedService,
    MessagingSeedService messagingSeedService,
    ILogger<SeedOrchestrator> logger)
{
    private readonly PatientPortalContext _context = context;
    private readonly StaffSeedService _staffSeedService = staffSeedService;
    private readonly PatientSeedService _patientSeedService = patientSeedService;
    private readonly MessagingSeedService _messagingSeedService = messagingSeedService;
    private readonly ILogger<SeedOrchestrator> _logger = logger;

    /// <summary>
    /// Seeds the database with the specified number of staff and patients, and messaging.
    /// </summary>
    public async Task SeedDatabaseAsync(int staffCount, int patientCount, bool seedMessages)
    {
        ConsoleWrites.WriteHeader();
        ConsoleWrites.WriteOperationParams(staffCount, patientCount, seedMessages);

        try
        {
            // Get current counts
            int currentStaff = await _context.Staff.CountAsync();
            int currentPatients = await _context.Patients.CountAsync();
            _logger.LogInformation("Database connected - Current: {Staff} staff, {Patients} patients",
                currentStaff, currentPatients);

            // Seed staff
            int seededStaff = 0;
            if (staffCount > 0)
            {
                seededStaff = await _staffSeedService.SeedStaffAsync(staffCount);
            }

            // Seed patients
            int seededPatients = 0;
            if (patientCount > 0)
            {
                seededPatients = await _patientSeedService.SeedPatientsAsync(patientCount);
            }

            // Seed messaging
            int seededPatientConversations = 0;
            int seededStaffConversations = 0;
            if (seedMessages)
            {
                (seededPatientConversations, seededStaffConversations) = await _messagingSeedService.SeedMessagingAsync();
            }

            // Display final summary
            int finalStaff = await _context.Staff.CountAsync();
            int finalPatients = await _context.Patients.CountAsync();

            ConsoleWrites.WriteOperationResults(seededStaff, seededPatients, seededPatientConversations, seededStaffConversations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seeding failed");
            throw;
        }
    }
}