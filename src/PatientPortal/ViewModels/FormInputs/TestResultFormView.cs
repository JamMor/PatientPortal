#nullable enable
// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class TestResultFormView
    {
        public required PatientHeaderInfoView Patient { get; set; }
        public required TestResultForm TestResultForm { get; set; }
    }

    public class TestResultForm
    {
        [Required]
        public string? Type { get; set; }

        [Required]
        [MinLength(5)]
        public string? Comment { get; set; }

        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }

    // TODO: Also used in VisitForm. Cleanup organization
    public class HealthIssueCheckbox
    {
        public int HealthIssueId { get; set; }
        public required string ShortDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Selected { get; set; } = false;
    }
}
