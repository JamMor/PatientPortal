using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class StafftoPatientConversation
    {
        [Key]
        public int S2PId { get; set; }
        public int MessagingPatient { get; set; }
        public List<Staff> MessagingStaff { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}