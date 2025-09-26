// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class VisitFormView
    {
        public required PatientHeaderInfoView Patient { get; set; }
        public required VisitForm VisitForm { get; set; }
    }

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
