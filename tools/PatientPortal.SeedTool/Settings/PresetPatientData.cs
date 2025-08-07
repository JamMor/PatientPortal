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

    private const string DefaultDomain = "colonialfleet.gov";

    public static readonly PatientPreset[] Patients =
    [
        new(
            FirstName: "William",
            LastName: "Adama",
            DOB: new DateTime(1952, 3, 15),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Hypertension",
                    LongDescription: "Sustained elevated blood pressure consistent with long-term occupational "
                        + "stress. Patient reports a history of irregular sleep patterns and "
                        + "high-stakes decision-making spanning several decades. Managed with "
                        + "lifestyle counseling; pharmacological intervention under review."
                ),
                new(
                    ShortDescription: "Coronary Artery Disease",
                    LongDescription: "Imaging reveals partial occlusion in the left anterior descending artery. "
                        + "Patient has a history of heavy smoking and a high-stress lifestyle. "
                        + "Ongoing cardiology referral recommended; aspirin therapy initiated."
                ),
            ]
        ),
        new(
            FirstName: "Laura",
            LastName: "Roslin",
            DOB: new DateTime(1958, 11, 5),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Breast Cancer, Stage III",
                    LongDescription: "Biopsy-confirmed invasive ductal carcinoma of the right breast. Patient "
                        + "presented with a self-detected lump during routine self-examination. "
                        + "Staged at III based on local lymph node involvement. Treatment plan "
                        + "includes combination chemotherapy and targeted hormone therapy."
                ),
                new(
                    ShortDescription: "Chemotherapy Side Effects",
                    LongDescription: "Patient currently undergoing chemotherapy regimen for breast cancer "
                        + "treatment. Presenting with fatigue, nausea, and peripheral neuropathy "
                        + "consistent with cytotoxic drug exposure. Antiemetics prescribed; "
                        + "nutritional support in progress."
                ),
            ]
        ),
        new(
            FirstName: "Kara",
            LastName: "Thrace",
            DOB: new DateTime(1985, 8, 22),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "PTSD - Combat",
                    LongDescription: "Patient presents with intrusive memories, hyperarousal, and avoidance "
                        + "behaviors consistent with post-traumatic stress disorder stemming from "
                        + "repeated combat exposure. Sleep disturbances and irritability reported. "
                        + "Referred for cognitive processing therapy."
                ),
                new(
                    ShortDescription: "Right Shoulder Tendinopathy",
                    LongDescription: "Chronic tendinopathy of the right rotator cuff attributed to repetitive "
                        + "overhead strain. Patient reports a history of high-intensity physical "
                        + "activity and combat-related trauma. Conservative management with "
                        + "physical therapy underway."
                ),
            ]
        ),
        new(
            FirstName: "Lee",
            LastName: "Adama",
            DOB: new DateTime(1983, 6, 10),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Anxiety Disorder",
                    LongDescription: "Patient reports persistent excessive worry, restlessness, and difficulty "
                        + "concentrating interfering with daily functioning. Symptoms have been "
                        + "present for over six months. Cognitive behavioral therapy initiated; "
                        + "medication review pending."
                ),
                new(
                    ShortDescription: "Lumbar Spine Injury",
                    LongDescription: "MRI reveals a herniated disc at L4-L5 with mild radiculopathy into the "
                        + "left leg. Patient attributes injury to repeated high-impact physical "
                        + "activity. Physical therapy prescribed; surgical consultation deferred."
                ),
            ]
        ),
        new(
            FirstName: "Gaius",
            LastName: "Baltar",
            DOB: new DateTime(1970, 2, 28),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Generalized Anxiety Disorder",
                    LongDescription: "Patient presents with chronic, excessive anxiety across multiple life "
                        + "domains, accompanied by muscle tension and difficulty concentrating. "
                        + "Symptoms have been present for over one year. SSRI therapy initiated "
                        + "with scheduled follow-up."
                ),
                new(
                    ShortDescription: "Auditory Hallucinations",
                    LongDescription: "Patient reports recurrent auditory experiences perceived as an external "
                        + "voice offering guidance and commentary. Neurological workup "
                        + "unremarkable. Psychiatric consultation ongoing; atypical antipsychotic "
                        + "therapy under consideration."
                ),
            ]
        ),
        new(
            FirstName: "Sharon",
            LastName: "Agathon",
            DOB: new DateTime(1988, 4, 17),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Immune System Irregularity",
                    LongDescription: "Patient exhibits atypical immune panel results inconsistent with standard "
                        + "population norms, including elevated autoantibodies. Clinical "
                        + "significance remains under investigation. Hematology referral in place."
                ),
                new(
                    ShortDescription: "Traumatic Stress Response",
                    LongDescription: "Acute stress response following an episode of severe physical trauma. "
                        + "Presenting with dissociation, emotional dysregulation, and somatic "
                        + "complaints. Short-term anxiolytic support prescribed; trauma-focused "
                        + "therapy recommended."
                ),
            ]
        ),
        new(
            FirstName: "Karl",
            LastName: "Agathon",
            DOB: new DateTime(1985, 12, 3),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Radiation Exposure - Acute",
                    LongDescription: "Patient experienced extended exposure to elevated ionizing radiation "
                        + "levels. Initial presentation included nausea and erythema; CBC "
                        + "now showing reduced leukocyte count. Hematological monitoring "
                        + "ongoing; prophylactic antibiotic therapy initiated."
                ),
                new(
                    ShortDescription: "Chronic Fatigue Syndrome",
                    LongDescription: "Patient reports persistent, debilitating fatigue unresolved by rest and "
                        + "lasting beyond six months, secondary to confirmed prior radiation "
                        + "exposure. Graded exercise therapy initiated alongside supportive "
                        + "care management."
                ),
            ]
        ),
        new(
            FirstName: "Saul",
            LastName: "Tigh",
            DOB: new DateTime(1948, 7, 19),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Alcohol Use Disorder",
                    LongDescription: "Patient meets diagnostic criteria for severe alcohol use disorder with "
                        + "a reported history of heavy daily consumption over several decades. "
                        + "Liver function tests indicate early hepatic compromise. "
                        + "Detoxification protocol initiated; long-term rehabilitation referral "
                        + "in progress."
                ),
                new(
                    ShortDescription: "Ocular Trauma - Left Eye",
                    LongDescription: "Patient presents with complete loss of vision in the left eye following "
                        + "blunt-force trauma. Ophthalmological exam confirms enucleation of the "
                        + "globe. Prosthetic fitting completed. Right eye maintained with "
                        + "regular monitoring."
                ),
            ]
        ),
        new(
            FirstName: "Galen",
            LastName: "Tyrol",
            DOB: new DateTime(1978, 9, 2),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Major Depressive Disorder",
                    LongDescription: "Patient presents with persistent low mood, anhedonia, psychomotor "
                        + "retardation, and passive suicidal ideation. Symptoms have markedly "
                        + "impaired occupational and social functioning. Antidepressant therapy "
                        + "initiated with close psychiatric follow-up."
                ),
                new(
                    ShortDescription: "Occupational Stress",
                    LongDescription: "Patient reports significant work-related stress characterized by "
                        + "exhaustion, cynicism, and reduced professional efficacy, consistent "
                        + "with occupational burnout. Reduced duty schedule recommended alongside "
                        + "stress management counseling."
                ),
            ]
        ),
        new(
            FirstName: "Samuel",
            LastName: "Anders",
            DOB: new DateTime(1984, 11, 16),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Traumatic Brain Injury",
                    LongDescription: "Patient sustained a penetrating cranial injury resulting in subdural "
                        + "hematoma with diffuse axonal involvement. Surgical decompression "
                        + "performed. Residual deficits include expressive aphasia and impaired "
                        + "executive function. Neurological rehabilitation ongoing."
                ),
                new(
                    ShortDescription: "Neural Pathway Damage",
                    LongDescription: "Imaging confirms disruption to multiple neural pathways in the frontal "
                        + "and parietal lobes following severe traumatic brain injury. Patient "
                        + "exhibits altered consciousness and complex motor dysfunction. "
                        + "Long-term prognosis under assessment."
                ),
            ]
        ),
        new(
            FirstName: "D'Anna",
            LastName: "Biers",
            DOB: new DateTime(1982, 5, 30),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Psychotic Disorder NOS",
                    LongDescription: "Patient presents with disorganized thought processes, delusional beliefs, "
                        + "and occasional auditory hallucinations. Symptoms do not fit a specific "
                        + "psychotic disorder category. Comprehensive psychiatric evaluation "
                        + "ongoing; antipsychotic medication trial initiated."
                ),
                new(
                    ShortDescription: "Substance-Induced Psychosis",
                    LongDescription: "Patient has a history of polysubstance use with episodes of acute psychosis "
                        + "temporally associated with substance intoxication. Current presentation "
                        + "includes psychotic symptoms without clear substance involvement. "
                        + "Dual diagnosis treatment approach recommended."
                ),
            ]
        ),
    ];
}
