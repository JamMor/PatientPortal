using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models;

public class VisitFormInput
{
    [Required]
    [MinLength(5)]
    public string? Comment { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Visit")]
    public DateTime DateOfVisit { get; set; } = DateTime.Today;

    public List<HealthIssueSelection> HealthIssues { get; set; } = [];
}
