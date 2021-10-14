using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class S2PStaffAssociation
    {
        [Key]
        public int S2PStaffAssociationId { get; set; }
        public int S2PId { get; set; }
        public int StaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //=======================================
        public StafftoPatientConversation StafftoPatientConversation { get; set; }
        public Staff Staff { get; set; }
    }
}