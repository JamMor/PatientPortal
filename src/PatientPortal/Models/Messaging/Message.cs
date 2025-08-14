#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public required string MessageText { get; set; }

        public int MessagingLinkId { get; set; }
        public int ConversationId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //========================================================

        public MessagingLink? Sender { get; set; }
        public Conversation? Conversation { get; set; }
        public List<Unread> UnreadBy { get; set; } = [];
    }
}
