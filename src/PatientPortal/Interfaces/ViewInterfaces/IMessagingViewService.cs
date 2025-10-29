using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingViewService
    {
        NewConversationFormView NewConversationForm(int linkId, int? toLinkId);
        MessageInboxView? ReturnInboxView(int linkId, InboxQuery query);
    }
}