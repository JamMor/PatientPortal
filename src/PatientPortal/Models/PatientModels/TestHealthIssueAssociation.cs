#nullable enable
using System;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class TestHealthIssueAssociation
    {
        [Key]
        public int TestHealthIssueAssociationId { get; set; }

        [Required]
        public int TestResultId { get; set; }

        [Required]
        public int HealthIssueId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Relationship Properties=============

        public TestResult? TestResult { get; set; }
        public HealthIssue? HealthIssue { get; set; }
    }
}
