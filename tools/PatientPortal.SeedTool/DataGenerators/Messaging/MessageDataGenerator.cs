using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators.Messaging;

/// <summary>
/// Generates fake Message data.
/// </summary>
public class MessageDataGenerator
{
    // Range of days after the earliest date for message creation to ensure 
    // messages threads are grouped in a realistic way.
    private static readonly int RangeOfDaysForMessageCreation = 10;

    /// <summary>
    /// Generates fake Message objects for a specific MessagingLinkId.
    /// </summary>
    /// <param name="count">Number of messages to generate</param>
    /// <param name="senderLinkId">ID of the sender's MessagingLink</param>
    /// <param name="earliestDate">Earliest possible message date</param>
    /// <returns>List of generated Message objects</returns>
    public List<Message> GenerateMessagesWithLinkId(int count, int senderLinkId, DateTime earliestDate)
    {
        DateTime maxDate = earliestDate.AddDays(RangeOfDaysForMessageCreation) < DateTime.Today
            ? earliestDate.AddDays(RangeOfDaysForMessageCreation)
            : DateTime.Today;

        return new Faker<Message>()
            .RuleFor(m => m.MessagingLinkId, senderLinkId)
            .RuleFor(m => m.MessageText, f => f.Lorem.Paragraph(1))
            .RuleFor(m => m.CreatedAt, f => f.Date.Between(earliestDate, maxDate))
            .RuleFor(m => m.UpdatedAt, (f, m) => m.CreatedAt)
            .Generate(count);
    }
}
