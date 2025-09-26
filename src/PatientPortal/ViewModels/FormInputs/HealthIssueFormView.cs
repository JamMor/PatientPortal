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
        public required HealthIssueForm HealthIssueForm { get; set; }
    }

    public class HealthIssueForm
    {
        [Required]
        [MaxLength(30)]
        [Display(Name = "Short Description")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Long Description")]
        public string? LongDescription { get; set; }
    }
}
