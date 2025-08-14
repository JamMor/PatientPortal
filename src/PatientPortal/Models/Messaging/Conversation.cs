#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// TODO: Consider an explicit PatientParticipant that is a one to one link. Thus
// a conversation can never have more than one patient involved if at all.
// This would also simplify the "WithPatient" field and inbox logic.
namespace PatientPortal.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }

        public bool WithPatient { get; set; }
        public string? Subject { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //=========================================================

        public List<ConversationParticipant> ConversationParticipants { get; set; } = [];
        public List<Message> Messages { get; set; } = [];
    }
}
