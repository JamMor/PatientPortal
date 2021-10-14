using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class StafftoPatientConversation
    {
        [Key]
        public int S2PId { get; set; }
        public int MessagingPatientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //============================================================
        public Patient MessagingPatient { get; set; }
        public List<S2PStaffAssociation> MessagingStaff { get; set; }
        public List<Message> Messages { get; set; }
    }
}