using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.MessageSeeding.DataGenerators;

/// <summary>
/// Generates fake Conversation data.
/// </summary>
public class ConversationDataGenerator
{
    // Furthest back electronic communications would go (10 years)
    private static readonly DateTime OldestAllowedDate = DateTime.Today.AddYears(-10);

    /// <summary>
    /// Generates a fake Conversation object with no participants or messages.
    /// Never older than 10 years ago.
    /// </summary>
    /// <param name="withPatient">Whether the conversation includes a patient</param>
    /// <param name="earliestDate">Earliest possible conversation date</param>
    /// <returns>Generated Conversation object</returns>
    public Conversation GenerateConversation(bool withPatient, DateTime earliestDate)
    {
        DateTime oldestPossible = earliestDate > OldestAllowedDate
            ? earliestDate
            : OldestAllowedDate;

        return new Faker<Conversation>()
            .RuleFor(c => c.WithPatient, withPatient)
            .RuleFor(c => c.Subject, f => f.Lorem.Sentence())
            .RuleFor(c => c.CreatedAt, f => f.Date.Between(oldestPossible, DateTime.Today.AddDays(-5)))
            .RuleFor(c => c.UpdatedAt, (f, c) => c.CreatedAt)
            .Generate();
    }
}
