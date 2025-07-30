namespace PatientPortal.SeedTool.DTOs.Messaging
{
    public class LinkConversationInfo
    {
        public required ParticipantInfo PrimaryLinkInfo;
        public required List<ParticipantInfo> PotentialCorrespondentInfos;
        public int ConversationCount;
    }

    public class ParticipantInfo
    {
        public int MessagingLinkId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}