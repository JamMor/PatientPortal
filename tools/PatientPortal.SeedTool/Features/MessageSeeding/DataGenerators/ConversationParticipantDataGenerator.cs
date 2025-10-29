using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.MessageSeeding.DataGenerators;

/// <summary>
/// Generates fake ConversationParticipant data.
/// </summary>
public class ConversationParticipantDataGenerator
{
    /// <summary>
    /// Generates a ConversationParticipant object for a given MessagingLinkId.
    /// </summary>
    /// <param name="messagingLinkId">MessagingLinkId</param>
    /// <param name="createdAt">Earliest possible CreatedAt date</param>
    /// <returns>ConversationParticipant object</returns>
    public ConversationParticipant GenerateConversationParticipant(
        int messagingLinkId,
        DateTime createdAt
    )
    {
        var faker = new Faker<ConversationParticipant>()
            .RuleFor(cp => cp.MessagingLinkId, messagingLinkId)
            .RuleFor(cp => cp.CreatedAt, createdAt)
            .RuleFor(cp => cp.UpdatedAt, (f, cp) => cp.CreatedAt);

        return faker.Generate(); // Generate one ConversationParticipant per MessagingLinkId
    }
}
