namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Preset patient seed data inspired by The Expanse characters.
/// </summary>
internal static class ExpansePatientData
{
    private const string DefaultDomain = "mcrn.mil";

    public static readonly PresetPatientData.PatientPreset[] Patients =
    [
        new(
            FirstName: "James",
            LastName: "Holden",
            DOB: new DateTime(1987, 5, 16),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Cumulative Radiation Exposure",
                    LongDescription: "Patient has accrued occupational radiation dose above recommended "
                        + "thresholds through repeated unshielded proximity to reactor "
                        + "cores during emergency operations. Annual oncological screening "
                        + "initiated; current CBC within normal limits. Long-term cancer "
                        + "risk counseling provided."
                ),
                new(
                    ShortDescription: "Moral Injury Syndrome",
                    LongDescription: "Patient presents with persistent guilt, self-condemnation, and "
                        + "difficulty reconciling past decisions made under extreme moral "
                        + "pressure. Symptoms include insomnia, social withdrawal, and "
                        + "compulsive re-evaluation of past events. Psychotherapy focused "
                        + "on values clarification initiated."
                ),
            ]
        ),
        new(
            FirstName: "Naomi",
            LastName: "Nagata",
            DOB: new DateTime(1988, 11, 4),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Osteopenia - Chronic",
                    LongDescription: "Patient demonstrates reduced bone mineral density and mild muscle "
                        + "weakness, consistent with chronic osteopenia due to limited weight-bearing "
                        + "activity. Fracture risk is elevated, especially at the hip and distal radius. "
                        + "Weight-bearing exercise regimen and bisphosphonate therapy initiated."
                ),
                new(
                    ShortDescription: "Hypoxic Episode - Recovered",
                    LongDescription: "Patient experienced a prolonged hypoxic event with residual exertional "
                        + "dyspnea and reduced diffusion capacity on pulmonary function testing. "
                        + "Supplemental oxygen available for exertion; pulmonology follow-up scheduled."
                ),
            ]
        ),
        new(
            FirstName: "Amos",
            LastName: "Burton",
            DOB: new DateTime(1984, 7, 28),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Blunt Force Polytrauma",
                    LongDescription: "Patient presents with healing contusions and fractures across multiple "
                        + "body regions consistent with repeated exposure to extreme physical "
                        + "force. Injuries appear at various stages of healing. Patient is "
                        + "non-distressed and minimizes symptom burden. Pain management "
                        + "and orthopedic follow-up arranged."
                ),
                new(
                    ShortDescription: "Affective Blunting - Evaluated",
                    LongDescription: "Patient demonstrates a consistently flat affect, reduced empathic "
                        + "response, and pragmatic approach to interpersonal situations. "
                        + "Full psychiatric evaluation completed; no acute psychotic features "
                        + "identified. Presentation assessed as stable characterological pattern. "
                        + "No pharmacological intervention indicated at this time."
                ),
            ]
        ),
        new(
            FirstName: "Alex",
            LastName: "Kamal",
            DOB: new DateTime(1980, 9, 29),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Hypertension - Stage II",
                    LongDescription: "Patient presents with persistently elevated blood pressure readings "
                        + "exceeding 160/100 mmHg on multiple measurements. Contributing factors "
                        + "include occupational stress, sedentary duty profile, and sodium-dense "
                        + "diet. ACE inhibitor initiated; dietary counseling provided."
                ),
                new(
                    ShortDescription: "Elevated Cerebrovascular Risk",
                    LongDescription: "Cardiovascular risk stratification indicates high ten-year risk of "
                        + "major cerebrovascular event based on age, hypertension, and elevated "
                        + "LDL cholesterol. Statin therapy initiated alongside antihypertensive "
                        + "regimen. Patient counseled on warning signs of acute stroke."
                ),
            ]
        ),
        new(
            FirstName: "Bobbie",
            LastName: "Draper",
            DOB: new DateTime(1991, 2, 17),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Combat Blast Injury",
                    LongDescription: "Patient sustained polytrauma including a right clavicle fracture, "
                        + "pulmonary contusion, and multiple soft tissue injuries during a "
                        + "high-yield explosive event in a hostile engagement. Surgical "
                        + "stabilization of the clavicle completed. Pulmonary status improving "
                        + "with supportive care."
                ),
                new(
                    ShortDescription: "PTSD - Sole Survivor",
                    LongDescription: "Patient is the sole survivor of a unit-level engagement in which "
                        + "all other personnel were killed. Presenting with survivor guilt, "
                        + "recurrent nightmares, and avoidance of discussion about the event. "
                        + "EMDR therapy initiated; peer support program recommended."
                ),
            ]
        ),
        new(
            FirstName: "Camina",
            LastName: "Drummer",
            DOB: new DateTime(1985, 4, 13),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Spinal Cord Injury - Partial",
                    LongDescription: "Patient sustained a penetrating spinal injury at the T6 level resulting "
                        + "in incomplete motor and sensory deficits in the lower extremities. "
                        + "Initial presentation required surgical stabilization. Significant "
                        + "neurological recovery has occurred with intensive rehabilitation; "
                        + "exoskeletal assist used for prolonged standing."
                ),
                new(
                    ShortDescription: "Chronic Neuropathic Pain",
                    LongDescription: "Patient reports burning, dysesthetic pain in bilateral lower extremities "
                        + "consistent with central sensitization following spinal cord injury. "
                        + "Pain is refractory to standard analgesics. Gabapentinoid therapy "
                        + "initiated; spinal cord stimulation evaluation pending."
                ),
            ]
        ),
        new(
            FirstName: "Chrisjen",
            LastName: "Avasarala",
            DOB: new DateTime(1960, 8, 22),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Hypertension - Stress-Related",
                    LongDescription: "Patient exhibits sustained elevated blood pressure consistent with "
                        + "chronic high-demand occupational stress. Non-pharmacological "
                        + "interventions have been partially effective. Calcium channel blocker "
                        + "added to existing regimen; repeat 24-hour ambulatory monitoring "
                        + "scheduled."
                ),
                new(
                    ShortDescription: "Osteoporosis",
                    LongDescription: "Patient demonstrates reduced bone mineral density with DEXA scan "
                        + "confirming osteoporotic changes at the lumbar spine and hip. "
                        + "Bisphosphonate therapy initiated; weight-bearing exercise regimen "
                        + "recommended."
                ),
                new(
                    ShortDescription: "Insomnia - Occupational Stress",
                    LongDescription: "Patient reports chronic difficulty initiating and maintaining sleep, "
                        + "attributed to persistent occupational stress and high-stakes decision making. "
                        + "Sleep hygiene counseling provided; short-term pharmacological intervention "
                        + "under consideration."
                ),
            ]
        ),
        new(
            FirstName: "Joseph",
            LastName: "Miller",
            DOB: new DateTime(1965, 10, 2),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Alcohol Use Disorder",
                    LongDescription: "Patient presents with a history of chronic alcohol consumption, "
                        + "manifesting as impaired sleep, mild hepatic enzyme elevation, and "
                        + "occasional cognitive slowing. Advised reduction in intake and regular "
                        + "monitoring of liver function. Counseling referral provided."
                ),
                new(
                    ShortDescription: "Adjustment Disorder",
                    LongDescription: "Patient reports persistent difficulty adapting to occupational stressors, "
                        + "including insomnia, irritability, and social withdrawal. Symptoms are "
                        + "exacerbated by high-pressure investigative work. Supportive therapy and "
                        + "stress management techniques recommended."
                ),
            ]
        ),
        new(
            FirstName: "Frederick",
            LastName: "Johnson",
            DOB: new DateTime(1958, 3, 11),
            EmailDomain: DefaultDomain,
            HealthIssues:
            [
                new(
                    ShortDescription: "Post-Traumatic Stress Disorder",
                    LongDescription: "Patient presents with recurrent nightmares, hypervigilance, and "
                        + "avoidance of reminders related to prior combat and leadership experiences. "
                        + "Symptoms have persisted for several years. Trauma-focused psychotherapy "
                        + "initiated; peer support program recommended."
                ),
                new(
                    ShortDescription: "Hypertension - Chronic",
                    LongDescription: "Patient exhibits sustained elevated blood pressure, likely related to "
                        + "long-term occupational stress and prior military service. Antihypertensive "
                        + "therapy initiated; regular monitoring advised."
                ),
            ]
        ),
    ];
}
