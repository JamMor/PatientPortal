using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Extensions;
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

        public MessageInboxView ReturnInboxView(int linkId, bool isPatientInbox, Paginator paginationSettings)
        {
            MessagingLink messageLink = _messagingService.GetMessagingLink(linkId);
            int unreadTotal = _messagingService.GetUnreadTotalCount(messageLink);
            int unreadPatient = _messagingService.GetUnreadPatientCount(messageLink);

            MessageInboxView inboxView = new MessageInboxView()
            {
                UnreadTotal = unreadTotal,
                UnreadPatient = unreadPatient,
                UnreadStaff = unreadTotal - unreadPatient,
                IsPatientInbox = isPatientInbox,
                PaginationSettings = paginationSettings
            };

            var conversations =  _messagingService.GetAllConversationsForInbox(linkId, isPatientInbox);
            inboxView.PaginationSettings.ResultsCount = conversations.Count();

            inboxView.Conversations = conversations
                .Select( c => new InboxConversation()
                {
                    ConversationId = c.ConversationId,
                    Subject = c.Subject,
                    Participating = c.ConversationParticipants
                        .Select(p => new InboxRecipient()
                        {
                            LinkId = p.MessagingLinkId,
                            Name = p.MessagingLink.UserType == "Patient" ? 
                                p.MessagingLink.Patient.FullName() : p.MessagingLink.Staff.FullName(),
                            Role = p.MessagingLink.UserType == "Patient" ? 
                                "Patient" : p.MessagingLink.Staff.Role
                        })
                        .ToList(),
                    Messages = c.Messages
                        .Select(m => new InboxMessage()
                        {
                            MessageId = m.MessageId,
                            SenderId = m.MessagingLinkId,
                            MessageText = m.MessageText,
                            Sent = m.CreatedAt,
                            Unread = m.UnreadBy
                                .Any(u => u.MessagingLinkId == linkId)
                        })
                        .OrderBy(m => m.Sent)
                        .ToList(),
                    DateCreated = c.CreatedAt,
                    DateLastMessage = c.UpdatedAt
                })
                .OrderByDescending(c => c.DateLastMessage)
                .ToPagedList(paginationSettings.ResultsPerPage, paginationSettings.CurrentPage);

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