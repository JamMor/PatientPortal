using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IMessagingViewService : IDisposable
    {
        // NewConversationFormView NewConversationForm(int linkId, int? toLinkId);
        MessageInboxView ReturnInboxView(int linkId, bool isPatientInbox);
    }
}