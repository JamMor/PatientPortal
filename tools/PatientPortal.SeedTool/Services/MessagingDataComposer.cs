using PatientPortal.Models;
using PatientPortal.SeedTool.DataGenerators.Messaging;

namespace PatientPortal.SeedTool.Services;

/// <summary>
/// Composes fully populated Conversation entities from messaging generators.
/// </summary>
public class MessagingDataComposer
{
    private readonly ConversationDataGenerator _conversationDataGenerator;
    private readonly MessageDataGenerator _messageDataGenerator;
    
    private const int MaxMessagesPerParticipant = 5;

    public MessagingDataComposer(
        ConversationDataGenerator conversationDataGenerator,
        MessageDataGenerator messageDataGenerator)
    {
        _conversationDataGenerator = conversationDataGenerator;
        _messageDataGenerator = messageDataGenerator;
    }

    public Conversation CreateConversationWithMessages(bool forPatient, List<int> participantIds, DateTime earliestDate)
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
