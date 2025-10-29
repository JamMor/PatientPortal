using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.Features.StaffSeeding.Services;
using PatientPortal.SeedTool.Settings;

namespace PatientPortal.SeedTool.Features.PresetSeeding.Services;

/// <summary>
/// Service for seeding a set of preset staff members into the database, each with an associated IdentityUser account.
/// If all preset staff are already present, seeding is skipped.
/// </summary>
public class PresetStaffSeedService(
    PatientPortalContext context,
    ILogger<PresetStaffSeedService> logger,
    PresetDataComposer presetDataComposer,
    StaffAccountService staffAccountService
)
{
    private readonly PatientPortalContext _context = context;
    private readonly ILogger<PresetStaffSeedService> _logger = logger;
    private readonly PresetDataComposer _presetDataComposer = presetDataComposer;
    private readonly StaffAccountService _staffAccountService = staffAccountService;

    /// <summary>
    /// Seeds all preset staff members into the database, creating IdentityUser accounts and linking them.
    /// If all preset staff are already present, no action is taken.
    /// </summary>
    /// <returns>
    /// The number of staff members created and seeded, or 0 if all preset staff already exist.
    /// </returns>
    public async Task<int> SeedAsync()
    {
        var staffToSeed = await GetUnseededPresetStaffAsync();

        if (staffToSeed.Count == 0)
        {
            _logger.LogInformation("All preset staff already seeded — skipping.");
            return 0;
        }

        var staffList = _presetDataComposer.CreatePresetStaff(staffToSeed);
        int successCount = 0;

        // Create IdentityUser for each staff member from first and last name and link them
        foreach (Staff staff in staffList)
        {
            var identityUser = await _staffAccountService.AttachAccountForStaffAsync(staff);

            if (identityUser != null)
            {
                staff.User = identityUser;
                _context.Staff.Add(staff);

                successCount++;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Preset staff seeding complete: {Count} created", successCount);
        return successCount;
    }

    /// <summary>
    /// Determines which preset staff members have not yet been seeded into the database
    /// by checking for their presence based on first and last name.
    /// </summary>
    /// <returns>
    /// A list of <see cref="PresetStaffData.StaffPreset"/> objects representing staff not present in the database.
    /// </returns>
    private async Task<List<PresetStaffData.StaffPreset>> GetUnseededPresetStaffAsync()
    {
        var allPresetStaff = PresetStaffData.Staff;

        var allExistingStaff = await _context.Staff
            .Select(s => new { s.FirstName, s.LastName })
            .ToListAsync();

        var existingSet = allExistingStaff
            .Select(e => (e.FirstName, e.LastName))
            .ToHashSet();

        var notInDb = allPresetStaff
            .Where(p => !existingSet
            .Contains((p.FirstName, p.LastName)))
            .ToList();

        return notInDb;
    }
}
