using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.MessageSeeding.DataGenerators;

/// <summary>
/// Generates fake Unread data.
/// </summary>
public class UnreadDataGenerator
{
    public Unread GenerateUnreadEntry(int messagingLinkId, bool withPatient)
    {
        return CreateFaker(messagingLinkId, withPatient).Generate();
    }

    private static Faker<Unread> CreateFaker(int messagingLinkId, bool withPatient)
    {
        var unreadFaker = new Faker<Unread>();

        return unreadFaker
            .RuleFor(u => u.MessagingLinkId, messagingLinkId)
            .RuleFor(u => u.WithPatient, withPatient);
    }
}
