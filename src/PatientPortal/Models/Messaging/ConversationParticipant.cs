using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class ConversationParticipant
    {
        [Key]
        public int ConversationParticipantId { get; set; }

        public int MessagingLinkId { get; set; }
        public int ConversationId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //====================================================================

        public MessagingLink? MessagingLink { get; set; }
        public Conversation? Conversation { get; set; }
    }
}
