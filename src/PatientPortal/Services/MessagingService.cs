using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
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

        public void CreateConversation(int linkId, ConversationDTO newConversationDTO, MessageDTO firstMessageDTO)
        {
            //Adds everyone to conversation including current user (sender)
            List<ConversationParticipant> conversationParticipants = newConversationDTO.RecipientLinkIds
                .Select(id => new ConversationParticipant() {MessagingLinkId = id})
                .ToList();
            conversationParticipants.Add(new ConversationParticipant() {MessagingLinkId = linkId});

            //Sets first message as unread for all other recipients
            List<Unread> unreadFor = newConversationDTO.RecipientLinkIds
                .Select(id => new Unread() {MessagingLinkId = id, WithPatient = newConversationDTO.WithPatient})
                .ToList();
            
            Conversation newConversation = new Conversation()
            {
                Subject = newConversationDTO.Subject,
                WithPatient = newConversationDTO.WithPatient,
                Messages = new List<Message>
                {
                    new Message()
                    {
                        MessageText = firstMessageDTO.MessageText,
                        MessagingLinkId = linkId,
                        UnreadBy = unreadFor
                    }
                },
                ConversationParticipants = conversationParticipants
            };
            
            _context.Conversations.Add(newConversation);
            _context.SaveChanges();
        }
        
        public void CreateReply(int linkId, int conversationId, MessageDTO newReply)
        {
            Conversation? thisConversation = _context
                .Conversations.Include(c => c.ConversationParticipants)
                .SingleOrDefault(c => c.ConversationId == conversationId);

            if (thisConversation == null) return;

            List<Unread> unreadFor = thisConversation
                .ConversationParticipants.Select(p => new Unread()
                {
                    MessagingLinkId = p.MessagingLinkId,
                    WithPatient = thisConversation.WithPatient,
                })
                .Where(p => p.MessagingLinkId != linkId)
                .ToList();

            Message newMessage = new Message()
            {
                MessagingLinkId = linkId,
                ConversationId = conversationId,
                MessageText = newReply.MessageText,
                UnreadBy = unreadFor,
            };

            _context.Messages.Add(newMessage);

            thisConversation.UpdatedAt = newMessage.CreatedAt;

            _context.SaveChanges();
        }
        
        public bool MarkRead(int linkId, int messageId)
        {
             Unread? unreadFlag = _context.UnreadMessages
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
        public MessagingLink? GetMessagingLink(int linkId)
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

        public Recipient? GetPatientRecipient(int? toLinkId)
        {
            return _context.MessagingLinks
                    .Include(m => m.Patient)
                    .Where(m => m.PatientId != null && m.MessagingLinkId == toLinkId)
                    .Select(m => new Recipient()
                        {
                            LinkId = m.MessagingLinkId,
                            Name = m.Patient!.FullName(),
                            Role = "Patient",
                            Selected = true
                        })
                    .FirstOrDefault();
        }

        public List<Recipient> GetAllOtherStaffAsRecipients(int linkId, int? toLinkId)
        {
            return _context.Staff
                .Include(staff => staff.MessagingLink)
                .Where(staff => staff.MessagingLink != null && staff.MessagingLink.MessagingLinkId != linkId)
                .OrderBy(staff => staff.Role)
                .ThenBy(staff => staff.LastName)
                .Select(staff => new Recipient()
                {
                    LinkId = staff.MessagingLink!.MessagingLinkId,
                    Name = staff.FullName(),
                    Role = staff.Role,
                    Selected = staff.MessagingLink.MessagingLinkId == toLinkId
                })
                .ToList();
        }
        
        public IQueryable<Conversation> GetAllConversationsForInbox(int linkId, ConversationSearch inboxFilters)
        {
            var conversations = _context.Conversations
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink!)
                        .ThenInclude(link => link.Patient)
                    .Include(convo => convo.ConversationParticipants)
                        .ThenInclude(partic => partic.MessagingLink!)
                        .ThenInclude(link => link.Staff)
                    .Include(convo => convo.Messages)
                        .ThenInclude(msg => msg.UnreadBy)
                    .AsSplitQuery()
                    .Where(convo => convo.ConversationParticipants
                        .Any(joined => joined.MessagingLinkId == linkId))
                    .Where(convo => convo.WithPatient == inboxFilters.IsPatientInbox);

            if(inboxFilters.OnlyUnread == true)
            {
                conversations = conversations
                    .Where(c => c.Messages.Any(m => m.UnreadBy.Any(u => u.MessagingLinkId == linkId)));
            }

            return conversations;
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