using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class MessagingService : IMessagingService
    {
        private PatientPortalContext _context;
        public MessagingService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS

        public void CreateConversation(int linkId, NewConversationFormView newConversationFormView)
        {
            //Gets Id's of everyone to receive message
            List<int> recipientIds = newConversationFormView.Recipients
                .Where(recipient => recipient.Selected == true)
                .Select(recipient => recipient.LinkId)
                .ToList();

            //Adds PatientId if present
            if(newConversationFormView.PatientRecipient != null)
            {
                recipientIds.Add(newConversationFormView.PatientRecipient.LinkId);
            }


            //Adds everyone to conversation including current user (sender)
            List<ConversationParticipant> conversationParticipants = recipientIds
                .Select(id => new ConversationParticipant() {MessagingLinkId = id})
                .ToList();
            conversationParticipants.Add(new ConversationParticipant() {MessagingLinkId = linkId});

            //Sets first message as unread for all other recipients
            List<Unread> unreadFor = recipientIds
                .Select(id => new Unread() {MessagingLinkId = id, WithPatient = newConversationFormView.WithPatient})
                .ToList();
            
            Conversation newConversation = new Conversation()
            {
                Subject = newConversationFormView.Subject,
                WithPatient = newConversationFormView.WithPatient,
                Messages = new List<Message>
                {
                    new Message()
                    {
                        MessageText = newConversationFormView.MessageText,
                        MessagingLinkId = linkId,
                        UnreadBy = unreadFor
                    }
                },
                ConversationParticipants = conversationParticipants
            };
            
            _context.Conversations.Add(newConversation);
            _context.SaveChanges();
        }
        
        public void CreateReply(int linkId, int conversationId, ReplyView newReply)
        {
            Conversation thisConversation = _context.Conversations
                    .Include(c => c.ConversationParticipants)
                    .SingleOrDefault(c => c.ConversationId == conversationId);

                List<Unread> unreadFor = thisConversation.ConversationParticipants
                    .Select(p => new Unread()
                    {
                        MessagingLinkId = p.MessagingLinkId,
                        WithPatient = thisConversation.WithPatient
                    })
                    .Where(p => p.MessagingLinkId != linkId)
                    .ToList();

                Message newMessage = new Message()
                {
                    MessagingLinkId = linkId,
                    ConversationId = conversationId,
                    MessageText = newReply.MessageText,
                    UnreadBy = unreadFor
                };

                _context.Messages.Add(newMessage);

                thisConversation.UpdatedAt = newMessage.CreatedAt;

                _context.SaveChanges();
        }
        
        public bool MarkRead(int linkId, int messageId)
        {
             Unread unreadFlag = _context.UnreadMessages
                .FirstOrDefault(u => u.MessagingLinkId == linkId && u.MessageId == messageId);
            
            if(unreadFlag != null)
            {
                _context.Remove(unreadFlag);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // QUERIES
        public MessagingLink GetMessagingLink(int linkId)
        {
            return _context.MessagingLinks
                .FirstOrDefault(link => link.MessagingLinkId == linkId);
        }
        
        public int GetUnreadTotalCount(MessagingLink messagingLink)
        {
            return _context.Entry(messagingLink)
                           .Collection(l => l.UnreadMessages)
                           .Query()
                           .TagWith("ServiceUnreadTotalQuery")
                           .Count();
        }
        
        public int GetUnreadPatientCount(MessagingLink messagingLink)
        {
            return _context.Entry(messagingLink)
                           .Collection(l => l.UnreadMessages)
                           .Query()
                           .Where(m =>m.WithPatient == true)
                           .TagWith("ServiceUnreadPatientQuery")
                           .Count();
        }

        public Recipient GetPatientRecipient(int? toLinkId)
        {
            return _context.MessagingLinks
                    .Include(m => m.Patient)
                    .Where(m => m.PatientId != null && m.MessagingLinkId == toLinkId)
                    .Select(m => new Recipient()
                        {
                            LinkId = m.MessagingLinkId,
                            Name = m.Patient.FullName(),
                            Role = "Patient",
                            Selected = true
                        })
                    .FirstOrDefault();
        }

        public List<Recipient> GetAllOtherStaffAsRecipients(int linkId, int? toLinkId)
        {
            return _context.Staff
                .Include(staff => staff.MessagingLink)
                .Where(staff => staff.MessagingLink.MessagingLinkId != linkId)
                .OrderBy(staff => staff.Role)
                .ThenBy(staff => staff.LastName)
                .Select(staff => new Recipient()
                {
                    LinkId = staff.MessagingLink.MessagingLinkId,
                    Name = staff.FullName(),
                    Role = staff.Role,
                    Selected = staff.MessagingLink.MessagingLinkId == toLinkId
                })
                .ToList();
        }
        
        public List<InboxConversation> GetAllConversationsForInbox(int linkId, bool isPatientInbox)
        {
            return _context.Conversations
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink)
                        .ThenInclude(link => link.Patient)
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink)
                        .ThenInclude(link => link.Staff)
                    .Include(convo => convo.Messages)
                        .ThenInclude(msg => msg.UnreadBy)
                    .Where(convo => convo.ConversationParticipants
                        .Any(joined => joined.MessagingLinkId == linkId))
                    .Where(convo => convo.WithPatient == isPatientInbox)
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
                    .ToList();
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
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