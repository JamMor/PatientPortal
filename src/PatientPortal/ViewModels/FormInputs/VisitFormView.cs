// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class VisitForm
    {
        [Required]
        [MinLength(5)]
        public string? Comment { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Visit")]
        public DateTime DateOfVisit { get; set; }

        public List<HealthIssueCheckbox> HealthIssues { get; set; } = [];
    }
}
