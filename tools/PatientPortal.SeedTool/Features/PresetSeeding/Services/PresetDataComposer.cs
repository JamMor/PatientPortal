using PatientPortal.Models;
using PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;
using PatientPortal.SeedTool.Features.PatientSeeding.Services;
using PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;
using PatientPortal.SeedTool.Settings;
using PatientPortal.SeedTool.Utilities;
using static PatientPortal.SeedTool.Settings.SeedSettings.PatientSettings;

namespace PatientPortal.SeedTool.Features.PresetSeeding.Services;

/// <summary>
/// Composes fully populated preset entities from preset data definitions and shared generators.
/// Handles Staff, Patient, and HealthIssue composition with associated Visits and TestResults.
/// </summary>
public class PresetDataComposer(
    StaffDataGenerator staffDataGenerator,
    PatientDataGenerator patientDataGenerator,
    PatientStaffConnectionDataGenerator connectionDataGenerator,
    HealthIssueDataGenerator healthIssueDataGenerator,
    PatientDataComposer patientDataComposer
)
{
    private readonly StaffDataGenerator _staffDataGenerator = staffDataGenerator;
    private readonly PatientDataGenerator _patientDataGenerator = patientDataGenerator;
    private readonly PatientStaffConnectionDataGenerator _connectionDataGenerator =
        connectionDataGenerator;
    private readonly HealthIssueDataGenerator _healthIssueDataGenerator = healthIssueDataGenerator;
    private readonly PatientDataComposer _patientDataComposer = patientDataComposer;

    /// <summary>
    /// Creates a <see cref="Staff"/> entity for every entry in <see cref="PresetStaffData.Staff"/>.
    /// IdentityUser creation is handled separately by the seed service.
    /// </summary>
    public List<Staff> CreatePresetStaff(List<PresetStaffData.StaffPreset> presetStaffToSeed) =>
        presetStaffToSeed
            .Select(p =>
                _staffDataGenerator.GenerateStaffWithDetails(p.FirstName, p.LastName, p.Role)
            )
            .ToList();

    /// <summary>
    /// Creates a <see cref="Patient"/> entity for every entry in <see cref="PresetPatientData.Patients"/>.
    /// Does not attach medical team or health issues.
    /// </summary>
    public List<Patient> CreatePresetPatients(
        List<PresetPatientData.PatientPreset> presetPatientsToSeed
    ) =>
        presetPatientsToSeed
            .Select(p =>
                _patientDataGenerator.GeneratePatientWithDetails(
                    p.FirstName,
                    p.LastName,
                    p.DOB,
                    p.Email
                )
            )
            .ToList();

    /// <summary>
    /// Assigns a random subset of available staff to a patient's medical team.
    /// </summary>
    /// <param name="patient">Patient to update (must have a valid <c>CreatedAt</c>).</param>
    /// <param name="availableStaffIds">Pool of staff IDs to draw from.</param>
    public void AttachMedicalTeam(Patient patient, List<int> availableStaffIds)
    {
        List<int> selectedIds = Rand.GetRandomSubset(
            availableStaffIds,
            Rand.BetweenOneAnd(MaxStaffPerPatient)
        );

        patient.MedicalTeam = _connectionDataGenerator.GenerateConnectionsForStaffIds(
            selectedIds,
            patient.CreatedAt
        );
    }

    /// <summary>
    /// Creates <see cref="HealthIssue"/> entities with associated <see cref="Visit"/> and
    /// <see cref="TestResult"/> records for a persisted patient.
    /// </summary>
    /// <param name="patient">
    ///   Persisted patient — must have a non-zero <c>PatientId</c> and a populated
    ///   <c>MedicalTeam</c> so that staff IDs are available.
    /// </param>
    /// <param name="presets">The patient's health issue preset definitions.</param>
    public List<HealthIssue> CreateHealthIssuesWithVisitsAndTests(
        Patient patient,
        PresetPatientData.HealthIssuePreset[] healthIssuePresetData
    )
    {
        List<int> medicalTeamIds = patient.MedicalTeam.Select(mt => mt.StaffId).ToList();

        List<HealthIssue> issues = healthIssuePresetData
            .Select(hi =>
                _healthIssueDataGenerator.GenerateFromDescriptions(
                    patient.PatientId,
                    patient.CreatedAt,
                    hi.ShortDescription,
                    hi.LongDescription
                )
            )
            .ToList();

        foreach (HealthIssue issue in issues)
        {
            _patientDataComposer.AttachHealthIssueAssociatedData(
                issue,
                patient.PatientId,
                medicalTeamIds
            );
        }

        return issues;
    }
}
