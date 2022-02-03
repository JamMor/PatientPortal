using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    [NotMapped]
    public class MessageInboxView
    {
        public int UnreadTotal { get; set; }
        public int UnreadStaff { get; set; }
        public int UnreadPatient { get; set; }
        public bool IsPatientInbox { get; set; }
        public List<InboxConversation> Conversations { get; set; }
    }

    [NotMapped]
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
    
    [NotMapped]
    public class InboxMessage
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string MessageText { get; set; }
        public DateTime Sent { get; set; }
        public bool Unread { get; set; }
    }

    [NotMapped]
    public class InboxRecipient
    {
        public int LinkId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}