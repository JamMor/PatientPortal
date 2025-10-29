namespace PatientPortal.Models;

public class HealthIssueCheckbox
{
    public int HealthIssueId { get; set; }
    public required string ShortDescription { get; set; }
    public bool Selected { get; set; } = false;
}
