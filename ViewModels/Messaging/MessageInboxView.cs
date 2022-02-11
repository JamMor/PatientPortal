using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    public class MessageInboxView
    {
        public int UnreadTotal { get; set; }
        public int UnreadStaff { get; set; }
        public int UnreadPatient { get; set; }

        public ConversationSearch InboxFilters { get; set; }

        public Paginator PaginationSettings { get; set; }

        public List<InboxConversation> Conversations { get; set; }
    }

    public class InboxConversation
    {
        public int ConversationId { get; set; }
        public string Subject { get; set; }
        public List<InboxRecipient> Participating { get; set; }
        public List<InboxMessage> Messages { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastMessage { get; set; }
        public int UnreadCount()
        {
            return Messages
                .Where(m => m.Unread == true)
                .Count();
        }
    }
    
    public class InboxMessage
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime Sent { get; set; }
        public bool Unread { get; set; }
    }

    public class InboxRecipient
    {
        public int LinkId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}