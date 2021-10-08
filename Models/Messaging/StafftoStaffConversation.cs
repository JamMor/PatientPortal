using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class StafftoStaffConversation
    {
        [Key]
        public int S2SConversationId { get; set; }
        public List<Staff> MessagingStaff { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}