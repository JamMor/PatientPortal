namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Preset patient seed data inspired by Firefly / Serenity characters.
/// </summary>
internal static class FireflyPatientData
{
    private const string DefaultDomain = "serenity.io";

    public static readonly PresetPatientData.PatientPreset[] Patients =
    [
        new(
            FirstName: "Malcolm",
            LastName: "Reynolds",
            DOB: new DateTime(1968, 9, 20),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "PTSD - Combat",
                    LongDescription: "Patient presents with hypervigilance, recurrent intrusive memories of "
                        + "battlefield engagements, and emotional numbing consistent with chronic "
                        + "post-traumatic stress disorder. Symptoms date to a decisive military "
                        + "defeat sustained years prior. Individual trauma therapy initiated; "
                        + "medication adjunct declined by patient."
                ),
                new(
                    ShortDescription: "Retained Shrapnel - Torso",
                    LongDescription: "Multiple small metallic fragments identified on imaging within the "
                        + "right lateral chest wall, consistent with historical projectile trauma. "
                        + "Fragments are stable and non-migratory. No surgical intervention "
                        + "indicated at this time; annual imaging recommended."
                ),
            ]
        ),
        new(
            FirstName: "Zoe",
            LastName: "Washburne",
            DOB: new DateTime(1972, 7, 14),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Hypertrophic Scarring - Trunk",
                    LongDescription: "Extensive hypertrophic scarring across the anterior trunk and left "
                        + "shoulder resulting from prior blast and fragment injuries sustained "
                        + "in active combat. Scar tissue is symptomatic with restricted range "
                        + "of motion. Silicone sheeting and physiotherapy recommended."
                ),
                new(
                    ShortDescription: "Adjustment Disorder - Grief",
                    LongDescription: "Patient presents with persistent depressed mood, social withdrawal, and "
                        + "occupational impairment following the traumatic death of a spouse. "
                        + "Symptoms exceed normal bereavement duration and intensity. "
                        + "Grief-focused psychotherapy initiated; antidepressant therapy pending review."
                ),
            ]
        ),
        new(
            FirstName: "Hoban",
            LastName: "Washburne",
            DOB: new DateTime(1970, 2, 20),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Occupational Stress - Pilot",
                    LongDescription: "Patient reports chronic stress and intermittent sleep disturbance "
                        + "related to high-responsibility piloting duties in hazardous environments. "
                        + "Symptoms include muscle tension, occasional headaches, and mild anxiety. "
                        + "Stress management techniques and regular rest periods recommended."
                ),
                new(
                    ShortDescription: "Repetitive Strain Injury",
                    LongDescription: "Patient experiences episodic wrist pain and stiffness, likely due to "
                        + "prolonged control manipulation and vibration exposure. Physical examination "
                        + "reveals mild tenderness over the carpal region. Advised ergonomic adjustments "
                        + "and periodic stretching exercises."
                ),
            ]
        ),
        new(
            FirstName: "Kaylee",
            LastName: "Frye",
            DOB: new DateTime(1987, 11, 30),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Penetrating Abdominal Wound",
                    LongDescription: "Patient sustained a single gunshot wound to the right lower quadrant "
                        + "with visceral involvement requiring emergency laparotomy. Bowel repair "
                        + "completed; post-operative course complicated by wound infection "
                        + "managed with targeted antibiotics. Patient recovering well."
                ),
                new(
                    ShortDescription: "Post-Surgical Adhesions",
                    LongDescription: "Patient reports intermittent cramping and abdominal discomfort attributed "
                        + "to intraperitoneal adhesion formation following prior emergency "
                        + "laparotomy. Symptoms are episodic and self-resolving. Dietary "
                        + "modification advised; surgical lysis deferred unless obstruction develops."
                ),
            ]
        ),
        new(
            FirstName: "Jayne",
            LastName: "Cobb",
            DOB: new DateTime(1973, 1, 12),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Occupational Blunt Trauma",
                    LongDescription: "Patient presents with multiple contusions, periorbital bruising, and "
                        + "lacerations consistent with repeated close-quarters physical altercations. "
                        + "No intracranial pathology identified. Wound care completed; patient "
                        + "counseled on protective measures, which were declined."
                ),
                new(
                    ShortDescription: "Stress Fracture - Right Radius",
                    LongDescription: "X-ray confirms a non-displaced stress fracture of the right distal "
                        + "radius consistent with repetitive loading of a heavy implement. "
                        + "Immobilization splint applied; return to full activity expected "
                        + "within six weeks pending repeat imaging."
                ),
            ]
        ),
        new(
            FirstName: "River",
            LastName: "Tam",
            DOB: new DateTime(1993, 6, 19),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Surgical Amygdala Modification",
                    LongDescription: "Neuroimaging reveals bilateral surgical ablation of the amygdalae "
                        + "with disruption of associated limbic circuitry. Patient demonstrates "
                        + "altered emotional processing, heightened threat sensitivity, and "
                        + "extraordinary pattern-recognition ability. No further intervention "
                        + "possible; ongoing psychiatric monitoring in place."
                ),
                new(
                    ShortDescription: "Dissociative Identity Disorder",
                    LongDescription: "Patient presents with fragmented identity, abrupt behavioral state "
                        + "changes, and intrusive somatic experiences with no clear environmental "
                        + "trigger. Presentation is consistent with severe dissociative disorder "
                        + "secondary to documented covert surgical trauma. Long-term structured "
                        + "psychotherapy recommended."
                ),
            ]
        ),
        new(
            FirstName: "Simon",
            LastName: "Tam",
            DOB: new DateTime(1990, 3, 25),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Chronic Sleep Deprivation",
                    LongDescription: "Patient reports averaging fewer than five hours of sleep per night "
                        + "over an extended period due to occupational demands and sustained "
                        + "situational stress. Presenting with cognitive slowing, irritability, "
                        + "and reduced fine motor precision. Sleep hygiene counseling provided."
                ),
                new(
                    ShortDescription: "Anxiety Disorder - Situational",
                    LongDescription: "Patient presents with persistent worry, somatic tension, and "
                        + "hypervigilance directly attributable to ongoing external threat "
                        + "circumstances. Symptoms are not disproportionate to the patient's "
                        + "objective situation. Supportive counseling provided; pharmacotherapy "
                        + "discussed but deferred at patient's request."
                ),
            ]
        ),
    ];
}
