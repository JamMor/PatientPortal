namespace PatientPortal.SeedTool.Settings;

/// <summary>
/// Centralized configuration constants for controlling the volume and distribution of seeded data.
/// Modify these values to adjust seeding behavior without touching individual classes.
/// </summary>
public static class SeedSettings
{
    public static class PatientSettings
    {
        /// <summary>Probability (0.0–1.0) that a patient will have an address record.</summary>
        public const double AddressProbability = 0.33;

        /// <summary>Maximum number of staff members assigned to a single patient's medical team.</summary>
        public const int MaxStaffPerPatient = 3;

        // Independent patient data (not linked to a specific HealthIssue)
        public const int MaxIndependentVisitsPerPatient = 3;
        public const int MaxIndependentTestResultsPerPatient = 2;
        public const int MaxIndependentHealthIssuesPerPatient = 2;

        // Health issue related data
        public const int MaxRelatedHealthIssuesPerPatient = 3;
        public const int MaxVisitsPerHealthIssue = 4;
        public const int MaxTestResultsPerHealthIssue = 3;
    }

    public static class MessagingSettings
    {
        /// <summary>Minimum number of conversations a MessagingLink must have before being skipped.</summary>
        public const int ConversationThreshold = 2;

        /// <summary>Maximum number of new conversations to create per MessagingLink when seeding.</summary>
        public const int MaxNewConversationsPerLink = (ConversationThreshold - 1) + 3;

        /// <summary>Maximum number of additional correspondents (beyond the primary link) per conversation.</summary>
        public const int MaxAdditionalCorrespondents = 2;

        /// <summary>Maximum number of messages generated per participant per conversation.</summary>
        public const int MaxMessagesPerParticipant = 5;
    }
}
