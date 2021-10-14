using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class S2SStaffAssociation
    {
        [Key]
        public int S2SStaffAssociationId { get; set; }
        public int S2SId { get; set; }
        public int StaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //====================================================================
        public StafftoStaffConversation StafftoStaffConversation { get; set; }
        public Staff Staff { get; set; }
    }
}