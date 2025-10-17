using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models;

public class TestResultFormInput
{
    [Required]
    public string? Type { get; set; }

    [Required]
    [MinLength(5)]
    public string? Comment { get; set; }

    public List<HealthIssueSelection> HealthIssues { get; set; } = [];
}
