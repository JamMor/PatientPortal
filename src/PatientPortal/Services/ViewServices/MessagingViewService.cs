using System.Linq;
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
                var patientRecipient = _messagingService.GetPatientRecipient(toLinkId);
                if(patientRecipient != null)
                {
                    newConversationFormViewModel.PatientRecipient = patientRecipient;
                }
            }

            return newConversationFormViewModel;
        }

        public MessageInboxView? ReturnInboxView(int linkId, ConversationSearch inboxFilters, Paginator paginationSettings)
        {
            MessagingLink? messageLink = _messagingService.GetMessagingLink(linkId);
            if (messageLink == null) return null;

            int unreadTotal = _messagingService.GetUnreadTotalCount(messageLink);
            int unreadPatient = _messagingService.GetUnreadPatientCount(messageLink);

            MessageInboxView inboxView = new MessageInboxView()
            {
                UnreadTotal = unreadTotal,
                UnreadPatient = unreadPatient,
                UnreadStaff = unreadTotal - unreadPatient,
                InboxFilters = inboxFilters,
                PaginationSettings = paginationSettings
            };

            var conversations =  _messagingService.GetAllConversationsForInbox(linkId, inboxFilters);
            inboxView.PaginationSettings.ResultsCount = conversations.Count();

            inboxView.Conversations = conversations
                .Select( c => new InboxConversation()
                {
                    ConversationId = c.ConversationId,
                    Subject = c.Subject,
                    UserLinkId = linkId,
                    StaffRecipients = c.ConversationParticipants
                        .Where(p => p.MessagingLink!.StaffId != null)
                        .Select(p => new InboxRecipient()
                        {
                            LinkId = p.MessagingLinkId,
                            Name = p.MessagingLink!.StaffId != null
                                    ? p.MessagingLink.Staff!.FullName()
                                    : "Unknown Staff",
                            Role = p.MessagingLink.StaffId != null
                                ? p.MessagingLink.Staff!.Role
                                : "Unknown Role"
                        })
                        .ToList(),
                    PatientRecipient = c.ConversationParticipants
                        .Where(p => p.MessagingLink!.PatientId != null)
                        .Select(p => new InboxRecipient()
                        {
                            LinkId = p.MessagingLinkId,
                            Name = p.MessagingLink!.PatientId != null
                                ? p.MessagingLink.Patient!.FullName()
                                : "Unknown Patient",
                            Role = "Patient"
                        })
                        .FirstOrDefault(),
                    UnknownRecipients = c.ConversationParticipants
                        .Where(p => p.MessagingLink!.PatientId == null && p.MessagingLink.StaffId == null)
                        .Select(p => new InboxRecipient()
                        {
                            LinkId = p.MessagingLinkId,
                            Name = "Unknown Recipient",
                            Role = "Unknown Role"
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
    }
}