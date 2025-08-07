using PatientPortal.Models;
using PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;
using PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;
using PatientPortal.SeedTool.Settings;

namespace PatientPortal.SeedTool.Features.PresetSeeding.Services;

/// <summary>
/// Composes fully populated preset entities from preset data definitions and shared generators.
/// Handles Staff composition.
/// </summary>
public class PresetDataComposer(
    StaffDataGenerator staffDataGenerator,
    PatientDataGenerator patientDataGenerator
)
{
    private readonly StaffDataGenerator _staffDataGenerator = staffDataGenerator;
    private readonly PatientDataGenerator _patientDataGenerator = patientDataGenerator;

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
}
