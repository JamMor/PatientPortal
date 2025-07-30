using PatientPortal.Models;
using PatientPortal.SeedTool.DataGenerators.Messaging;
using PatientPortal.SeedTool.DTOs.Messaging;

namespace PatientPortal.SeedTool.Services;

/// <summary>
/// Composes fully populated Conversation entities from messaging generators.
/// </summary>
public class MessagingDataComposer
{
    private readonly ConversationDataGenerator _conversationDataGenerator;
    private readonly MessageDataGenerator _messageDataGenerator;

    //TODO: Fix duplication
    private const int ConversationThreshold = 2;
    // private const int MaxNewConversationsPerLink = 4;
    private const int MaxNewConversationsPerLink = (ConversationThreshold - 1) + 3;

    private const int MaxAdditionalCorrespondents = 2;
    private const int MaxMessagesPerParticipant = 5;

    public MessagingDataComposer(
        ConversationDataGenerator conversationDataGenerator,
        MessageDataGenerator messageDataGenerator)
    {
        _conversationDataGenerator = conversationDataGenerator;
        _messageDataGenerator = messageDataGenerator;
    }

    public List<Conversation> CreateConversationsForPatients(List<LinkConversationInfo> patientConversationInfos) =>
        CreateConversationsWithMessages(patientConversationInfos, forPatient: true);
    public List<Conversation> CreateConversationsForStaffToStaff(List<LinkConversationInfo> patientConversationInfos) =>
        CreateConversationsWithMessages(patientConversationInfos, forPatient: false);
    
    private List<Conversation> CreateConversationsWithMessages(List<LinkConversationInfo> conversationInfos, bool forPatient)
    {
        List<Conversation> conversations = new List<Conversation>();

        foreach (LinkConversationInfo info in conversationInfos)
        {
            int minimumToReachThreshold = ConversationThreshold - info.ConversationCount;
            int conversationsToCreate = Random.Shared.Next(minimumToReachThreshold, MaxNewConversationsPerLink + 1);

            for (int i = 0; i < conversationsToCreate; i++)
            {
                var participants = SelectConversationParticipants(info.PrimaryLinkInfo, info.PotentialCorrespondentInfos);

                List<int> participantIds = participants.Select(p => p.MessagingLinkId).ToList();
                DateTime mostRecentCreatedDate = participants.Max(p => p.CreatedAt);

                var conversation = CreateConversationWithMessages(forPatient, participantIds, mostRecentCreatedDate);

                conversations.Add(conversation);
            }
        }
        return conversations;
    }

    private static List<ParticipantInfo> SelectConversationParticipants(ParticipantInfo primaryInfo, List<ParticipantInfo> potentialCorrespondents)
    {
        List<ParticipantInfo> correspondents = GetRandomSubset(potentialCorrespondents, BetweenOneAnd(MaxAdditionalCorrespondents));

        return correspondents.Append(primaryInfo).ToList();
    }

    private Conversation CreateConversationWithMessages(bool forPatient, List<int> participantIds, DateTime earliestDate)
    {
        var conversation = _conversationDataGenerator.GenerateConversation(forPatient, earliestDate);
        List<Message> messages;
        List<ConversationParticipant> conversationParticipants;
        (messages, conversationParticipants) = CreateMessagesAndParticipants(participantIds, earliestDate);

        conversation.Messages = messages;
        conversation.ConversationParticipants = conversationParticipants;
        return conversation;
    }

    private (List<Message>, List<ConversationParticipant>) CreateMessagesAndParticipants(
        List<int> participantIds, DateTime firstMessageTime)
    {
        List<Message> messages = CreateInitialMessageInList(firstMessageTime, participantIds);
        List<ConversationParticipant> conversationParticipants = [];

        foreach (var participantId in participantIds)
        {
            int messageCount = BetweenOneAnd(MaxMessagesPerParticipant);
            var participantMessages = _messageDataGenerator.GenerateMessagesWithLinkId(
                messageCount,
                participantId,
                firstMessageTime
                );


            var conversationParticipant = new ConversationParticipant
            {
                MessagingLinkId = participantId,
                CreatedAt = participantMessages.Min(m => m.CreatedAt)
            };

            conversationParticipants.Add(conversationParticipant);
            messages.AddRange(participantMessages);
        }

        return (messages, conversationParticipants);
    }

    private List<Message> CreateInitialMessageInList(DateTime createdAt, List<int> participantIds)
    {
        int randomParticipantId = GetRandomSubset(participantIds, 1).Single();

        var listWithInitialMessage = _messageDataGenerator.GenerateMessagesWithLinkId(
            1, randomParticipantId, createdAt);
        listWithInitialMessage[0].CreatedAt = createdAt;

        return listWithInitialMessage;
    }

    // TODO: Fix Duplication
    private static int BetweenOneAnd(int max)
    {
        return Random.Shared.Next(1, max + 1);
    }

    private static List<T> GetRandomSubset<T>(List<T> list, int count)
    {
        return list.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
    }
}
