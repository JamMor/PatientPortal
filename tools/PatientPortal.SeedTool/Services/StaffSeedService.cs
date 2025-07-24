using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.DataGenerators;

namespace PatientPortal.SeedTool.Services;

internal static class IdentityErrorCodes
{
    private static readonly IdentityErrorDescriber Describer = new();
    
    public static readonly string DuplicateUserName = Describer.DuplicateUserName("").Code;

}

/// <summary>
/// Service for seeding fake staff data with IdentityUser accounts into the database.
/// </summary>
public class StaffSeedService
{
    private readonly PatientPortalContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly StaffDataGenerator _staffDataGenerator;
    private readonly IdentityUserDataGenerator _identityUserDataGenerator;
    private readonly ILogger<StaffSeedService> _logger;

    public StaffSeedService(
        PatientPortalContext context, 
        UserManager<IdentityUser> userManager,
        StaffDataGenerator staffDataGenerator,
        IdentityUserDataGenerator identityUserDataGenerator,
        ILogger<StaffSeedService> logger)
    {
        _context = context;
        _userManager = userManager;
        _staffDataGenerator = staffDataGenerator;
        _identityUserDataGenerator = identityUserDataGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Seeds a specified number of fake staff members with IdentityUser accounts.
    /// </summary>
    /// <param name="staffCount">Number of staff members to create</param>
    /// <returns>Number of successfully created staff members</returns>
    public async Task<int> SeedStaffAsync(int staffCount)
    {
        if (staffCount <= 0) return 0;
        int successCount = 0;
        int failCount = 0;

        _logger.LogInformation("Generating {Count} staff members...", staffCount);

        // Generate all staff data upfront using pure data generator
        var staffList = _staffDataGenerator.GenerateNStaff(staffCount);

        // Create IdentityUser for each staff member and link them
        for (int i = 0; i < staffList.Count; i++)
        {
            var staff = staffList[i];

            // Create IdentityUser based on staff's first and last name
            var identityUser = await CreateIdentityUserAsync(staff.FirstName, staff.LastName);

            if (identityUser == null)
            {
                _logger.LogWarning("Skipping staff member '{FirstName} {LastName}' due to IdentityUser creation failure", 
                    staff.FirstName, staff.LastName);
                failCount++;
                continue;
            }

            // Link Staff to IdentityUser
            staff.User = identityUser;
            _context.Staff.Add(staff);
            successCount++;
        }

        // Save all staff records to database in one operation
        await _context.SaveChangesAsync();

        if (failCount > 0)
        {
            _logger.LogWarning("Staff seeding complete: {Success} created, {Failed} failed", successCount, failCount);
        }
        else
        {
            _logger.LogInformation("Staff seeding complete: {Success} created", successCount);
        }
        
        return successCount;
    }

    /// <summary>
    /// Creates an IdentityUser account based on staff member's name.
    /// Retries once on duplicate username errors.
    /// </summary>
    /// <param name="firstName">Staff member's first name</param>
    /// <param name="lastName">Staff member's last name</param>
    /// <returns>Created IdentityUser or null if creation failed</returns>
    private async Task<IdentityUser?> CreateIdentityUserAsync(string firstName, string lastName)
    {
        var userProps = _identityUserDataGenerator.GenerateIdentityUserProperties(firstName, lastName);

        // Try creating user, with one retry on duplicate username
        IdentityUser identityUser = new IdentityUser { UserName = userProps.Username };
        var result = await _userManager.CreateAsync(identityUser, userProps.Password);
        
        // Retry once with a new username if duplicate
        if (result.Errors.Any(e => e.Code == IdentityErrorCodes.DuplicateUserName))
        {
            _logger.LogWarning("Duplicate username '{Username}' for '{FirstName} {LastName}', retrying", 
                identityUser.UserName, firstName, lastName);
            
            userProps = _identityUserDataGenerator.GenerateIdentityUserProperties(firstName, lastName);
            identityUser.UserName = userProps.Username;
            result = await _userManager.CreateAsync(identityUser, userProps.Password);
        }
        
        if (result.Succeeded)
        {
            return identityUser;
        }

        // Log detailed error information
        _logger.LogError("Failed to create IdentityUser for '{FirstName} {LastName}': {Errors}", 
            firstName, lastName, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
        
        return null;
    }
}