namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Aggregator for all preset staff seed data, combined from theme-specific source files
/// under <c>Settings/Staff/</c>. Add new theme files there and include them in
/// <see cref="Staff"/> to expand the preset staff pool.
/// </summary>
public static class PresetStaffData
{
    /// <summary>
    /// Represents a single preset staff member's identifying information.
    /// </summary>
    /// <param name="FirstName">Staff member's first name.</param>
    /// <param name="LastName">Staff member's last name.</param>
    /// <param name="Role">Clinical role abbreviation: MD, RN, NP, or LPN.</param>
    public record StaffPreset(string FirstName, string LastName, string Role);

    /// <summary>
    /// All preset staff members, aggregated from every theme-specific data file.
    /// </summary>
    public static readonly StaffPreset[] Staff =
    [
        .. StarTrekStaffData.Staff,
    ];
}
