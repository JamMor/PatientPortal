namespace PatientPortal.SeedTool.DTOs.Messaging
{
    public class ConversationDTO
    {
        public required ParticipantDTO PrimaryLinkInfo;
        public required List<ParticipantDTO> PotentialCorrespondentInfos;
        public int ConversationCount;
    }

    public class ParticipantDTO
    {
        public int MessagingLinkId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}