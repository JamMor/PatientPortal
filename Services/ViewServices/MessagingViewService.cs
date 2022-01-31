using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class MessagingViewService : IMessagingViewService
    {
        private IMessagingService _messagingService;
        public MessagingViewService(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        //COMMANDS
        
        public NewConversationFormView NewConversationForm(int linkId, int? toLinkId)
        {
            NewConversationFormView newConversationFormViewModel = new NewConversationFormView()
            {
                Recipients = _messagingService.GetAllOtherStaffAsRecipients(linkId, toLinkId),
            };

            //Adds patient recipient if toLinkId(addressee) is a patient
            if(toLinkId != null)
            {
                newConversationFormViewModel.PatientRecipient = _messagingService.GetPatientRecipient(toLinkId);
            }

            return newConversationFormViewModel;
        }

        public MessageInboxView ReturnInboxView(int linkId, bool isPatientInbox)
        {
            MessagingLink messageLink = _messagingService.GetMessagingLink(linkId);
            int unreadTotal = _messagingService.GetUnreadTotalCount(messageLink);
            int unreadPatient = _messagingService.GetUnreadPatientCount(messageLink);

            MessageInboxView inboxView = new MessageInboxView()
            {
                UnreadTotal = unreadTotal,
                UnreadPatient = unreadPatient,
                UnreadStaff = unreadTotal -unreadPatient,
                IsPatientInbox = isPatientInbox,
                Conversations = _messagingService.GetAllConversationsForInbox(linkId, isPatientInbox)
            };

            return inboxView;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _messagingService.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}