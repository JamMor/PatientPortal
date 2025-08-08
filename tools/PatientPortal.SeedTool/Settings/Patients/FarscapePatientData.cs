namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Preset patient seed data inspired by Farscape characters.
/// </summary>
internal static class FarscapePatientData
{
    private const string DefaultDomain = "moya.net";

    public static readonly PresetPatientData.PatientPreset[] Patients =
    [
        new(
            FirstName: "John",
            LastName: "Crichton",
            DOB: new DateTime(1969, 4, 19),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Neural Implant Sequelae",
                    LongDescription: "Patient presents with chronic headaches, cognitive intrusions, and "
                        + "intermittent lapses in executive function consistent with residual "
                        + "effects of a foreign neural implant. Neuroimaging reveals focal "
                        + "scar tissue at the insertion site. Neurosurgical consultation "
                        + "ongoing; implant confirmed inactive."
                ),
                new(
                    ShortDescription: "PTSD - Prolonged Captivity",
                    LongDescription: "Patient meets full diagnostic criteria for post-traumatic stress disorder "
                        + "following a prolonged period of captivity involving repeated physical "
                        + "and psychological torture. Presenting with hypervigilance, dissociative "
                        + "episodes, and distorted body image. Trauma-focused CBT initiated."
                ),
                new(
                    ShortDescription: "Psychotic and Dissociative Features",
                    LongDescription: "Patient reports persistent experiences of vivid internal dialogue and the "
                        + "sensation of an autonomous internal presence. Symptoms include episodes of derealization, "
                        + "intrusive thoughts, and difficulty distinguishing internal from external stimuli. "
                        + "No clear evidence of substance use or neurological disorder. "
                        + "Differential includes dissociative identity phenomena and primary psychotic spectrum disorder. "
                        + "Outpatient psychiatric follow-up recommended."
                ),
            ]
        ),
        new(
            FirstName: "Aeryn",
            LastName: "Sun",
            DOB: new DateTime(1975, 3, 8),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Pulmonary Contusion",
                    LongDescription: "Patient sustained blunt-force chest trauma resulting in bilateral "
                        + "pulmonary contusion with reduced oxygen diffusion capacity. "
                        + "Spirometry confirms moderate restrictive pattern. Supplemental "
                        + "oxygen therapy initiated; full recovery anticipated with rest."
                ),
                new(
                    ShortDescription: "Combat-Related Joint Damage",
                    LongDescription: "Recurrent trauma to the knees and right hip consistent with sustained "
                        + "high-intensity combat activity over many years. Imaging reveals early "
                        + "degenerative changes at the right hip joint and bilateral patellar "
                        + "erosion. Physical therapy initiated; joint replacement deferred."
                ),
                new(
                    ShortDescription: "Heat Intolerance",
                    LongDescription: "Patient reports persistent intolerance to elevated temperatures, "
                        + "manifesting as excessive sweating, fatigue, and irritability in warm "
                        + "environments. Symptoms are suggestive of possible thyroid dysfunction; "
                        + "thyroid function tests recommended. Advised to avoid heat exposure and "
                        + "maintain adequate hydration."
                ),
            ]
        ),
        new(
            FirstName: "Ka",
            LastName: "D'Argo",
            DOB: new DateTime(1968, 6, 14),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Episodic Rage Disorder",
                    LongDescription: "Patient reports recurrent episodes of uncontrollable rage with partial "
                        + "amnesia and physical aggression toward objects and bystanders. Episodes "
                        + "correlate with perceived threats or elevated stress. Mood stabilizer "
                        + "initiated; de-escalation behavioral therapy recommended."
                ),
                new(
                    ShortDescription: "Penetrating Abdominal Trauma",
                    LongDescription: "Patient sustained a penetrating laceration to the right upper quadrant "
                        + "in a prior combat engagement. Surgical repair completed with "
                        + "subsequent infection requiring antibiotic irrigation. Wound has "
                        + "healed; hepatic function tests remain within normal limits."
                ),
            ]
        ),
        new(
            FirstName: "Zhaan",
            LastName: "Delvian",
            DOB: new DateTime(1960, 12, 1),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Acute Radiation Sickness",
                    LongDescription: "Patient was exposed to an intense solar radiation event without adequate "
                        + "shielding. Presenting with nausea, diffuse alopecia, and a significant "
                        + "drop in white cell count. Supportive care underway; hematological "
                        + "monitoring scheduled at weekly intervals."
                ),
                new(
                    ShortDescription: "Psychogenic Dissociation",
                    LongDescription: "Patient experiences recurring dissociative episodes characterized by "
                        + "depersonalization and profound emotional detachment. Episodes are "
                        + "precipitated by high-stimulus environments. History of prior "
                        + "institutionalization noted. Mindfulness-based therapy initiated."
                ),
            ]
        ),
        new(
            FirstName: "Chiana",
            LastName: "Nebari",
            DOB: new DateTime(1985, 9, 23),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Transient Visual Disturbance",
                    LongDescription: "Patient reports recurrent episodes of complete transient blindness "
                        + "lasting seconds to minutes, followed by a period of heightened "
                        + "spatial awareness. No structural ocular pathology identified on "
                        + "examination. Neurology referral placed; etiology under investigation."
                ),
                new(
                    ShortDescription: "Conversion Disorder",
                    LongDescription: "Patient presents with episodic motor weakness and sensory loss in the "
                        + "lower extremities without corresponding neurological lesion. Symptoms "
                        + "are temporally associated with acute psychosocial stressors. "
                        + "Psychiatric consultation initiated alongside physiotherapy."
                ),
            ]
        ),
        new(
            FirstName: "Rygel",
            LastName: "XVI",
            DOB: new DateTime(1955, 2, 5),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Metabolic Syndrome",
                    LongDescription: "Patient meets criteria for metabolic syndrome with central adiposity, "
                        + "elevated fasting glucose, dyslipidemia, and hypertension. Lifestyle "
                        + "modification counseling provided; metformin therapy under "
                        + "consideration pending endocrinology review."
                ),
                new(
                    ShortDescription: "Diabetes Mellitus",
                    LongDescription: "Patient exhibits persistent hyperglycemia, polyuria, and polydipsia. "
                        + "Hemoglobin A1c is elevated above diagnostic threshold. Dietary "
                        + "modification and glucose monitoring recommended; pharmacologic "
                        + "therapy under consideration."
                ),
                new(
                    ShortDescription: "Hyperuricemia (Gout)",
                    LongDescription: "Patient reports episodic joint pain and swelling, most notably in the "
                        + "lower extremities. Serum uric acid levels are elevated. Symptoms "
                        + "consistent with gouty arthritis. Advised on dietary purine restriction; "
                        + "uric acid-lowering therapy to be considered if attacks persist."
                ),
            ]
        ),
    ];
}
