#nullable enable
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

public static class NewConversationFormViewExtensions
{
    public static ConversationDTO ToConversationDTO(this NewConversationFormView form)
    {
        var recipientIds = form.Recipients
            .Where(r => r.Selected)
            .Select(r => r.LinkId)
            .ToList();

        if (form.PatientRecipient != null)
        {
            recipientIds.Add(form.PatientRecipient.LinkId);
        }

        return new ConversationDTO(
            form.Subject,
            form.WithPatient,
            recipientIds
        );
    }

    public static MessageDTO ToMessageDTO(this NewConversationFormView form)
    {
        return new MessageDTO(
            Guard.NotNull(form.MessageText, nameof(form.MessageText))
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