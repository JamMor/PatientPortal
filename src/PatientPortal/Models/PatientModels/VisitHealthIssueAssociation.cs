using System;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class VisitHealthIssueAssociation
    {
        [Key]
        public int VisitHealthIssueAssociationId { get; set; }

        [Required]
        public int VisitId { get; set; }

        [Required]
        public int HealthIssueId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Relationship Properties=============

        public Visit? Visit { get; set; }
        public HealthIssue? HealthIssue { get; set; }
    }
}
