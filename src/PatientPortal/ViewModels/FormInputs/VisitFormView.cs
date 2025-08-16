#nullable enable
// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class VisitFormView
    {
        public Visit? Visit { get; set; }
        public required PatientHeaderInfoView Patient { get; set; }
        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }
}
