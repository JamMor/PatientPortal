#nullable enable
// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class HealthIssueFormView
    {
        public required PatientHeaderInfoView Patient { get; set; }
        public HealthIssue? HealthIssue { get; set; }
    }
}
