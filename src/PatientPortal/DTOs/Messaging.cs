using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record ConversationDTO(
    string? Subject,
    bool WithPatient,
    List<int> RecipientLinkIds
);

public record MessageDTO(
    string MessageText
);

public static class NewConversationFormInputExtensions
{
    public static ConversationDTO ToConversationDTO(this NewConversationFormInput input)
    {
        var recipientIds = input.Recipients
            .Where(r => r.Selected)
            .Select(r => r.LinkId)
            .ToList();

        if (input.PatientRecipient != null)
        {
            recipientIds.Add(input.PatientRecipient.LinkId);
        }

        return new ConversationDTO(
            input.Subject,
            input.WithPatient,
            recipientIds
        );
    }

    public static MessageDTO ToMessageDTO(this NewConversationFormInput input)
    {
        return new MessageDTO(
            Guard.NotNull(input.MessageText, nameof(input.MessageText))
        );
    }
}

public static class ReplyViewExtensions
{
    public static MessageDTO ToMessageDTO(this ReplyView form)
    {
        return new MessageDTO(
            Guard.NotNull(form.MessageText, nameof(form.MessageText))
        );
    }
}