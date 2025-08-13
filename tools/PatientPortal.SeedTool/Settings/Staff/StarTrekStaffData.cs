namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Preset Star Trek seed staff data.
/// </summary>
internal static class StarTrekStaffData
{
    public static readonly PresetStaffData.StaffPreset[] Staff =
    [
        // TOS
        new("James", "Kirk", "MD", true),
        new("Leonard", "McCoy", "MD", true),
        new("Nyota", "Uhura", "RN"),
        new("Hikaru", "Sulu", "LPN"),
        // TNG
        new("Jean-Luc", "Picard", "MD", true),
        new("William", "Riker", "PA-C"),
        new("Beverly", "Crusher", "MD"),
        new("Alyssa", "Ogawa", "RN"),
        new("Katherine", "Pulaski", "MD"),
        new("Deanna", "Troi", "NP"),
        new("Data", "Soong", "MD", true),
        new("Geordi", "La Forge", "MLS"),
        new("Worf", "Rozhenko", "LPN"),
        new("Reginald", "Barclay", "MD"),
        // DS9
        new("Benjamin", "Sisko", "MD", true),
        new("Julian", "Bashir", "MD"),
        new("Jadzia", "Dax", "NP"),
        new("Kira", "Nerys", "RN"),
        new("Miles", "O'Brien", "LPN"),
        new("Elim", "Garak", "MLS"),
        // Voyager
        new("Kathryn", "Janeway", "MD", true),
    ];
}
