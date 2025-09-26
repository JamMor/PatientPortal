using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingViewService : IDisposable
    {
        NewConversationFormView NewConversationForm(int linkId, int? toLinkId);
        MessageInboxView? ReturnInboxView(int linkId, ConversationSearch inboxFilters, Paginator paginationSettings);
    }
}