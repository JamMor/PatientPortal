namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Preset Star Trek seed staff data.
/// </summary>
internal static class StarTrekStaffData
{
    public static readonly PresetStaffData.StaffPreset[] Staff =
    [
        // TNG
        new("Jean-Luc", "Picard", "MD"),
        new("William", "Riker", "PA-C"),
        new("Beverly", "Crusher", "MD"),
        new("Deanna", "Troi", "NP"),
        new("Data", "Soong", "MD"),
        new("Geordi", "La Forge", "MLS"),
        new("Worf", "Rozhenko", "LPN"),
    ];
}
