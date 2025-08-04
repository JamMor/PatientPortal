using Microsoft.Extensions.Logging;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.StaffSeeding.Services;

/// <summary>
/// Service for seeding fake staff data with IdentityUser accounts into the database.
/// </summary>
public class StaffSeedService(
    PatientPortalContext context,
    ILogger<StaffSeedService> logger,
    StaffDataComposer staffDataComposer
)
{
    private readonly PatientPortalContext _context = context;
    private readonly ILogger<StaffSeedService> _logger = logger;
    private readonly StaffDataComposer _staffDataComposer = staffDataComposer;

    /// <summary>
    /// Seeds a specified number of fake staff members with IdentityUser accounts.
    /// </summary>
    /// <param name="staffCount">Number of staff members to create</param>
    /// <returns>Number of successfully created staff members</returns>
    public async Task<int> SeedStaffAsync(int staffCount)
    {
        if (staffCount <= 0)
            return 0;
        int successCount = 0;

        _logger.LogInformation("Generating {Count} staff members...", staffCount);

        // Generate all staff data
        var staffList = _staffDataComposer.CreateStaff(staffCount);

        // Create IdentityUser for each staff member from first and last name and link them
        foreach (Staff staff in staffList)
        {
            var identityUser = await _staffDataComposer.CreateIdentityUserForStaff(staff);

            if (identityUser == null)
            {
                continue;
            }

            _context.Attach(identityUser); // Attach to context so EF Core tracks it without trying to insert again

            staff.User = identityUser;
            _context.Staff.Add(staff);

            successCount++;
        }

        // Save all staff records to database in one operation
        await _context.SaveChangesAsync();

        _logger.LogInformation("Staff seeding complete: {Success} created", successCount);

        return successCount;
    }
}
