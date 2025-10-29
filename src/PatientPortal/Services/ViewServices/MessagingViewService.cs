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
            if (toLinkId != null)
            {
                var patientRecipient = _messagingService.GetPatientRecipient(toLinkId);
                if (patientRecipient != null)
                {
                    newConversationFormViewModel.PatientRecipient = patientRecipient;
                }
            }

            return newConversationFormViewModel;
        }

        public MessageInboxView? ReturnInboxView(int linkId, InboxQuery query)
        {
            MessagingLink? messageLink = _messagingService.GetMessagingLink(linkId);
            if (messageLink == null) return null;

            int unreadTotal = _messagingService.GetUnreadTotalCount(messageLink);
            int unreadPatient = _messagingService.GetUnreadPatientCount(messageLink);
            int unreadStaff = unreadTotal - unreadPatient;

            var conversationQuery = query.Type.WithPatient
                ? _messagingService.GetPatientConversations(linkId, query.OnlyUnread)
                : _messagingService.GetStaffConversations(linkId, query.OnlyUnread);

            MessageInboxView inboxView = new MessageInboxView()
            {
                UnreadTotal = unreadTotal,
                Query = query,
                Tabs =
                [
                    new InboxTab(InboxType.Patient, IsActive: query.Type == InboxType.Patient, unreadPatient),
                    new InboxTab(InboxType.Staff, IsActive: query.Type == InboxType.Staff, unreadStaff),
                ],
            };

            inboxView.Query.Paging.ResultsCount = conversationQuery.Count();

            inboxView.Conversations = conversationQuery
                .Select(c => new InboxConversation()
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
                                    : "Unknown Role",
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
                            Role = "Patient",
                        })
                        .FirstOrDefault(),
                    UnknownRecipients = c.ConversationParticipants
                        .Where(p =>
                            p.MessagingLink!.PatientId == null 
                            && p.MessagingLink.StaffId == null
                        )
                        .Select(p => new InboxRecipient()
                        {
                            LinkId = p.MessagingLinkId,
                            Name = "Unknown Recipient",
                            Role = "Unknown Role",
                        })
                        .ToList(),
                    Messages = c.Messages
                        .Select(m => new InboxMessage()
                        {
                            MessageId = m.MessageId,
                            SenderId = m.MessagingLinkId,
                            MessageText = m.MessageText,
                            Sent = m.CreatedAt,
                            Unread = m.UnreadBy.Any(u => u.MessagingLinkId == linkId),
                        })
                        .OrderBy(m => m.Sent)
                        .ToList(),
                    DateCreated = c.CreatedAt,
                    DateLastMessage = c.UpdatedAt,
                })
                .OrderByDescending(c => c.DateLastMessage)
                .ToPagedList(query.Paging.ResultsPerPage, query.Paging.CurrentPage);

            return inboxView;
        }
    }
}
