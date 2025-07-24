using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;
using PatientPortal.SeedTool.Services;

namespace PatientPortal.SeedTool;

/// <summary>
/// Orchestrates the database seeding process.
/// </summary>
public class SeedOrchestrator
{
    private readonly PatientPortalContext _context;
    private readonly StaffSeedService _staffSeedService;

    public SeedOrchestrator(PatientPortalContext context, StaffSeedService staffSeedService)
    {
        _context = context;
        _staffSeedService = staffSeedService;
    }

    /// <summary>
    /// Seeds the database with the specified number of staff and patients.
    /// </summary>
    public async Task SeedDatabaseAsync(int staffCount, int patientCount)
    {
        // TODO: Implement actual seeding logic here
        Console.WriteLine("=== PatientPortal Seed Tool ===");
        Console.WriteLine($"Staff to create: {staffCount}");
        Console.WriteLine($"Patients to create: {patientCount}");
        Console.WriteLine();
        Console.WriteLine("================================================");

        try
        {
            // Test connection by getting current counts
            int currentStaff = await _context.Staff.CountAsync();
            int currentPatients = await _context.Patients.CountAsync();
                
            Console.WriteLine($"✓ Connected to database");
            Console.WriteLine($"  Current Staff: {currentStaff}");
            Console.WriteLine($"  Current Patients: {currentPatients}");
            Console.WriteLine();

            if (staffCount > 0)
            {
                await _staffSeedService.SeedStaffAsync(staffCount);
                Console.WriteLine();
            }
            if (patientCount > 0)
            {
                // TODO: Uncomment when PatientSeedService is implemented
            }

            // Display final counts
            int finalStaff = await _context.Staff.CountAsync();
            int finalPatients = await _context.Patients.CountAsync();
            Console.WriteLine("Final counts:");
            Console.WriteLine($"  Total Staff: {finalStaff} (+{finalStaff - currentStaff})");
            Console.WriteLine($"  Total Patients: {finalPatients} (+{finalPatients - currentPatients})");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"✗ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.Error.WriteLine($"  Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}