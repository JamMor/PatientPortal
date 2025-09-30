// Form Input Model
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class TestResultForm
    {
        [Required]
        public string? Type { get; set; }

        [Required]
        [MinLength(5)]
        public string? Comment { get; set; }

        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }
}
