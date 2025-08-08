using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PatientPortal.Models;
using PatientPortal.SeedTool.Settings;

namespace PatientPortal.SeedTool.Features.PresetSeeding.Services;

/// <summary>
/// Seeds all preset patients with health issues, visits, and test results.
/// Skips seeding if all preset patients are already present in the database.
/// </summary>
public class PresetPatientSeedService(
    PatientPortalContext context,
    ILogger<PresetPatientSeedService> logger,
    PresetDataComposer presetDataComposer
)
{
    private readonly PatientPortalContext _context = context;
    private readonly ILogger<PresetPatientSeedService> _logger = logger;
    private readonly PresetDataComposer _presetDataComposer = presetDataComposer;

    /// <summary>
    /// Seeds all preset patients. Returns the number of patients created, or 0 if skipped.
    /// </summary>
    public async Task<int> SeedAsync()
    {
        var patientsToSeed = await GetUnseededPresetPatientsAsync();
        if (patientsToSeed.Count == 0)
        {
            _logger.LogInformation("All preset patients already seeded — skipping.");
            return 0;
        }

        // Prefer preset staff as medical team pool; fall back to any available staff.
        List<int> staffIds = await GetPresetStaffIdsAsync();
        if (staffIds.Count == 0)
        {
            _logger.LogWarning(
                "No preset staff found in the database. Falling back to all available staff."
            );
            staffIds = await _context.Staff.Select(s => s.StaffId).ToListAsync();
        }

        if (staffIds.Count == 0)
        {
            _logger.LogError(
                "No staff members found in the database. Cannot seed preset patients without staff."
            );
            return 0;
        }

        // Phase 1 — create patients and attach medical teams, then persist to obtain PatientIds.
        List<Patient> patients = _presetDataComposer.CreatePresetPatients(patientsToSeed);
        foreach (Patient patient in patients)
            _presetDataComposer.AttachMedicalTeam(patient, staffIds);

        _context.Patients.AddRange(patients);
        await _context.SaveChangesAsync();

        // Phase 2 — create health issues with associated visits and test results.
        // Zip preserves insertion order so each patient maps to its preset definition.
        //TODO: Continue work here
        List<HealthIssue> healthIssues = patients
            .Zip(
                patientsToSeed,
                (patient, preset) =>
                    _presetDataComposer.CreateHealthIssuesWithVisitsAndTests(
                        patient,
                        preset.HealthIssues
                    )
            )
            .SelectMany(issues => issues)
            .ToList();

        _context.HealthIssues.AddRange(healthIssues);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Preset patient seeding complete: {Count} patients, {IssueCount} health issues created",
            patients.Count,
            healthIssues.Count
        );

        return patients.Count;
    }

    private async Task<List<PresetPatientData.PatientPreset>> GetUnseededPresetPatientsAsync()
    {
        var allPresetPatients = PresetPatientData.Patients;

        var allExistingPatients = await _context.Patients
            .Select(p => new { p.FirstName, p.LastName })
            .ToListAsync();

        var existingSet = allExistingPatients
            .Select(e => (e.FirstName, e.LastName))
            .ToHashSet();

        var notInDb = allPresetPatients
            .Where(p => !existingSet
            .Contains((p.FirstName, p.LastName)))
            .ToList();

        return notInDb;
    }

    private async Task<List<int>> GetPresetStaffIdsAsync()
    {
        var presetStaffSet = PresetStaffData.Staff
            .Select(p => (p.FirstName, p.LastName))
            .ToHashSet();

        var allExistingStaff = await _context.Staff
            .Select(s => new
            {
                s.StaffId,
                s.FirstName,
                s.LastName,
            })
            .ToListAsync();

        var existingPresetStaffIds = allExistingStaff
            .Where(s => presetStaffSet.Contains((s.FirstName, s.LastName)))
            .Select(s => s.StaffId)
            .ToList();

        return existingPresetStaffIds;
    }
}
