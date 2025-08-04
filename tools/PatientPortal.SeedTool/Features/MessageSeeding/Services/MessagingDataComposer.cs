using PatientPortal.Models;
using PatientPortal.SeedTool.Features.MessageSeeding.DataGenerators;
using PatientPortal.SeedTool.Features.MessageSeeding.DTOs;
using PatientPortal.SeedTool.Utilities;
using static PatientPortal.SeedTool.Settings.SeedSettings.MessagingSettings;

namespace PatientPortal.SeedTool.Features.MessageSeeding.Services;

/// <summary>
/// Composes fully populated Conversation entities from messaging generators.
/// </summary>
public class MessagingDataComposer(
    ConversationDataGenerator conversationDataGenerator,
    MessageDataGenerator messageDataGenerator
)
{
    private readonly ConversationDataGenerator _conversationDataGenerator = conversationDataGenerator;
    private readonly MessageDataGenerator _messageDataGenerator = messageDataGenerator;

    public List<Conversation> CreateConversationsForPatients(
        List<ConversationDTO> patientConversationInfos
    ) => CreateConversationsWithMessages(patientConversationInfos, forPatient: true);

    public List<Conversation> CreateConversationsForStaffToStaff(
        List<ConversationDTO> patientConversationInfos
    ) => CreateConversationsWithMessages(patientConversationInfos, forPatient: false);

    private List<Conversation> CreateConversationsWithMessages(
        List<ConversationDTO> conversationInfos,
        bool forPatient
    )
    {
        List<Conversation> conversations = [];

        foreach (ConversationDTO info in conversationInfos)
        {
            int minimumToReachThreshold = ConversationThreshold - info.ConversationCount;
            int conversationsToCreate = Rand.Between(
                minimumToReachThreshold,
                MaxNewConversationsPerLink
            );

            for (int i = 0; i < conversationsToCreate; i++)
            {
                var participants = SelectConversationParticipants(
                    info.PrimaryLinkInfo,
                    info.PotentialCorrespondentInfos
                );

                List<int> participantIds = participants.Select(p => p.MessagingLinkId).ToList();
                DateTime mostRecentCreatedDate = participants.Max(p => p.CreatedAt);

                var conversation = CreateConversationWithMessages(
                    forPatient,
                    participantIds,
                    mostRecentCreatedDate
                );

                conversations.Add(conversation);
            }
        }
        return conversations;
    }

    private static List<ParticipantDTO> SelectConversationParticipants(
        ParticipantDTO primaryInfo,
        List<ParticipantDTO> potentialCorrespondents
    )
    {
        List<ParticipantDTO> correspondents = Rand.GetRandomSubset(
            potentialCorrespondents,
            Rand.BetweenOneAnd(MaxAdditionalCorrespondents)
        );

        return correspondents.Append(primaryInfo).ToList();
    }

    private Conversation CreateConversationWithMessages(
        bool forPatient,
        List<int> participantIds,
        DateTime earliestDate
    )
    {
        var conversation = _conversationDataGenerator.GenerateConversation(
            forPatient,
            earliestDate
        );
        List<Message> messages;
        List<ConversationParticipant> conversationParticipants;
        (messages, conversationParticipants) = CreateMessagesAndParticipants(
            participantIds,
            earliestDate
        );

        conversation.Messages = messages;
        conversation.ConversationParticipants = conversationParticipants;
        return conversation;
    }

    private (List<Message>, List<ConversationParticipant>) CreateMessagesAndParticipants(
        List<int> participantIds,
        DateTime firstMessageTime
    )
    {
        List<Message> messages = CreateInitialMessageInList(firstMessageTime, participantIds);
        List<ConversationParticipant> conversationParticipants = [];

        foreach (var participantId in participantIds)
        {
            int messageCount = Rand.BetweenOneAnd(MaxMessagesPerParticipant);
            var participantMessages = _messageDataGenerator.GenerateMessagesWithLinkId(
                messageCount,
                participantId,
                firstMessageTime
            );

            var conversationParticipant = new ConversationParticipant
            {
                MessagingLinkId = participantId,
                CreatedAt = participantMessages.Min(m => m.CreatedAt),
            };

            conversationParticipants.Add(conversationParticipant);
            messages.AddRange(participantMessages);
        }

        return (messages, conversationParticipants);
    }

    private List<Message> CreateInitialMessageInList(DateTime createdAt, List<int> participantIds)
    {
        int randomParticipantId = Rand.GetRandomElement(participantIds);

        var listWithInitialMessage = _messageDataGenerator.GenerateMessagesWithLinkId(
            1,
            randomParticipantId,
            createdAt
        );
        listWithInitialMessage[0].CreatedAt = createdAt;

        return listWithInitialMessage;
    }
}
