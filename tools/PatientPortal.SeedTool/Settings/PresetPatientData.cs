namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Aggregator for all preset patient seed data, combined from theme-specific source files
/// under <c>Settings/Patients/</c>. Add new theme files there and include them in
/// <see cref="Patients"/> to expand the preset patient pool.
/// </summary>
public static class PresetPatientData
{
    /// <summary>
    /// Represents a single health issue belonging to a preset patient.
    /// </summary>
    /// <param name="ShortDescription">Brief clinical label (max 30 chars).</param>
    /// <param name="LongDescription">Full clinical narrative.</param>
    public record HealthIssuePreset(string ShortDescription, string LongDescription);

    /// <summary>
    /// Represents a single preset patient's demographic and clinical data.
    /// </summary>
    /// <param name="FirstName">Patient's first name.</param>
    /// <param name="LastName">Patient's last name.</param>
    /// <param name="DOB">Date of birth.</param>
    /// <param name="EmailDomain">Domain used to generate the patient's email address.</param>
    /// <param name="HealthIssues">Pre-defined health issues for this patient.</param>
    public record PatientPreset(
        string FirstName,
        string LastName,
        DateTime DOB,
        string EmailDomain,
        HealthIssuePreset[] HealthIssues
    )
    {
        /// <summary>
        /// Auto-generated email address derived from the patient's name and theme domain.
        /// </summary>
        public string Email => $"{FirstName.ToLower()}.{LastName.ToLower()}@{EmailDomain}";
    }

    /// <summary>
    /// All preset patients, aggregated from every theme-specific data file.
    /// </summary>
    public static readonly PatientPreset[] Patients =
    [
        .. BattlestarPatientData.Patients,
        .. FarscapePatientData.Patients,
        .. FireflyPatientData.Patients,
        .. ExpansePatientData.Patients,
    ];
}
