using Microsoft.Extensions.Logging;

namespace PatientPortal.SeedTool.Features.PresetSeeding.Services;

/// <summary>
/// Orchestrates preset seeding by coordinating <see cref="PresetStaffSeedService"/>
/// and <see cref="PresetPatientSeedService"/>.
/// Triggered by the <c>--presets</c> CLI flag.
/// </summary>
public class PresetSeedService(
    ILogger<PresetSeedService> logger,
    PresetStaffSeedService presetStaffSeedService,
    PresetPatientSeedService presetPatientSeedService
)
{
    private readonly ILogger<PresetSeedService> _logger = logger;
    private readonly PresetStaffSeedService _presetStaffSeedService = presetStaffSeedService;
    private readonly PresetPatientSeedService _presetPatientSeedService = presetPatientSeedService;

    /// <summary>
    /// Seeds preset staff and patients.
    /// </summary>
    /// <returns>A tuple of (seeded staff count, seeded patient count).</returns>
    public async Task<(int SeededStaff, int SeededPatients)> SeedPresetsAsync()
    {
        _logger.LogInformation("Starting preset seeding...");
        int seededStaff = await _presetStaffSeedService.SeedAsync();
        int seededPatients = await _presetPatientSeedService.SeedAsync();
        return (seededStaff, seededPatients);
    }
}
