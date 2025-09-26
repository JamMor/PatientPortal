using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class TestResult
    {
        [Key]
        public int TestResultId { get; set; }

        [Required]
        public required string Type { get; set; }

        [Required]
        [MinLength(5)]
        public required string Comment { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int StaffId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Relationship Properties=============

        public Patient? Patient { get; set; }
        public Staff? Staff { get; set; }
        public List<TestHealthIssueAssociation> AssociatedHealthIssues { get; set; } = [];
    }
}
