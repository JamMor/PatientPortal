using System;
using System.Collections.Generic;
using System.Linq;

namespace PatientPortal.Models
{
    public class MessageInboxView
    {
        public int UnreadTotal { get; set; }
        public int UnreadStaff { get; set; }
        public int UnreadPatient { get; set; }

        public required ConversationSearch InboxFilters { get; set; }

        public required Paginator PaginationSettings { get; set; }

        public List<InboxConversation> Conversations { get; set; } = [];
    }

    public class InboxConversation
    {
        public int ConversationId { get; set; }
        public string? Subject { get; set; }
        public int UserLinkId { get; set; }
        public List<InboxRecipient> StaffRecipients { get; set; } = [];
        public InboxRecipient? PatientRecipient { get; set; }
        public List<InboxRecipient> UnknownRecipients { get; set; } = [];
        public List<InboxMessage> Messages { get; set; } = [];
        public DateTime DateCreated { get; set; }
        public DateTime DateLastMessage { get; set; }

        // The primary display recipient: patient takes priority, otherwise first non-viewer staff member.
        public InboxRecipient? PrimaryRecipient =>
            PatientRecipient ?? StaffRecipients.FirstOrDefault(p => p.LinkId != UserLinkId);

        // Count of all recipients beyond the primary, for the overflow badge.
        public int OtherRecipientsCount =>
            StaffRecipients.Count(p => p.LinkId != UserLinkId) + UnknownRecipients.Count;

        // All recipient names ordered for the tooltip (patient first, then other staff, then unknowns).
        public string RecipientNameString
        {
            get
            {
                var names = StaffRecipients
                    .Where(p => p.LinkId != UserLinkId)
                    .Select(p => p.Name)
                    .ToList();
                names.AddRange(UnknownRecipients.Select(r => r.Name));
                if (PatientRecipient != null) names.Insert(0, PatientRecipient.Name);
                return string.Join(", ", names);
            }
        }

        public string GetSenderName(int senderId)
        {
            IEnumerable<InboxRecipient> all = StaffRecipients.Concat(UnknownRecipients);
            if (PatientRecipient != null) all = all.Append(PatientRecipient);
            return all.FirstOrDefault(p => p.LinkId == senderId)?.Name ?? "(unknown)";
        }

        public int UnreadCount()
        {
            return Messages.Where(m => m.Unread == true).Count();
        }
    }

    public class InboxMessage
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public required string MessageText { get; set; }
        public DateTime Sent { get; set; }
        public bool Unread { get; set; }
    }

    public class InboxRecipient
    {
        public int LinkId { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; }
    }
}
