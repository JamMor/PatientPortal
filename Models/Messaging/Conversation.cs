using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }
        public bool WithPatient { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //=========================================================
        public List<ConversationParticipant> ConversationParticipants { get; set; }
        public List<Message> Messages { get; set; }
    }
}