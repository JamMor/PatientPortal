using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;

namespace PatientPortal.SeedTool.Features.StaffSeeding.Services;

/// <summary>
/// Composes staff data and manages IdentityUser creation for staff seeding.
/// </summary>
public class StaffDataComposer(
    PatientPortalContext context,
    UserManager<IdentityUser> userManager,
    StaffDataGenerator staffDataGenerator,
    IdentityUserDataGenerator identityUserDataGenerator,
    ILogger<StaffDataComposer> logger
)
{
    private readonly PatientPortalContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly StaffDataGenerator _staffDataGenerator = staffDataGenerator;
    private readonly IdentityUserDataGenerator _identityUserDataGenerator =
        identityUserDataGenerator;
    private readonly ILogger<StaffDataComposer> _logger = logger;

    /// <summary>
    /// Generates a list of fake staff members.
    /// </summary>
    /// <param name="count">Number of staff members to create</param>
    /// <returns>List of generated Staff objects</returns>
    public List<Staff> CreateStaff(int count)
    {
        return _staffDataGenerator.GenerateNStaff(count);
    }

    /// <summary>
    /// Creates an IdentityUser account for the given staff member.
    /// </summary>
    /// <param name="staff">Staff member to create an IdentityUser for</param>
    /// <returns>Created IdentityUser or null if creation failed</returns>
    public async Task<IdentityUser?> CreateIdentityUserForStaff(Staff staff)
    {
        var identityUser = await CreateIdentityUserAsync(staff.FirstName, staff.LastName);

        if (identityUser == null)
        {
            _logger.LogWarning(
                "Skipping staff member '{FirstName} {LastName}' due to IdentityUser creation failure",
                staff.FirstName,
                staff.LastName
            );
        }

        return identityUser;
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
        var userProps = _identityUserDataGenerator.GenerateIdentityUserProperties(
            firstName,
            lastName
        );

        // Try creating user, with one retry on duplicate username
        IdentityUser identityUser = new IdentityUser { UserName = userProps.Username };
        var result = await _userManager.CreateAsync(identityUser, userProps.Password);

        // Retry once with a new username if duplicate
        if (result.Errors.Any(e => e.Code == IdentityErrorCodes.DuplicateUserName))
        {
            _logger.LogWarning(
                "Duplicate username '{Username}' for '{FirstName} {LastName}', retrying",
                identityUser.UserName,
                firstName,
                lastName
            );

            userProps = _identityUserDataGenerator.GenerateIdentityUserProperties(
                firstName,
                lastName
            );
            identityUser.UserName = userProps.Username;
            result = await _userManager.CreateAsync(identityUser, userProps.Password);
        }

        if (result.Succeeded)
        {
            return identityUser;
        }

        // Log detailed error information
        _logger.LogError(
            "Failed to create IdentityUser for '{FirstName} {LastName}': {Errors}",
            firstName,
            lastName,
            string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))
        );

        return null;
    }

    internal static class IdentityErrorCodes
    {
        private static readonly IdentityErrorDescriber Describer = new();

        public static readonly string DuplicateUserName = Describer.DuplicateUserName("").Code;
    }
}
