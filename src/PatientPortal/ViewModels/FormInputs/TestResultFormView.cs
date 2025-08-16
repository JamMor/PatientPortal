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
        public TestResult? TestResult { get; set; }
        public required PatientHeaderInfoView Patient { get; set; }
        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }

    public class HealthIssueCheckbox
    {
        public int HealthIssueId { get; set; }
        public required string ShortDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Selected { get; set; } = false;
    }
}
