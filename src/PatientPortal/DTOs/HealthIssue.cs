using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record HealthIssueDTO(
    string ShortDescription,
    string? LongDescription
);

public static class HealthIssueFormViewExtensions
{
    public static HealthIssueDTO ToHealthIssueDTO(this HealthIssueForm healthIssueForm)
    {
        return new HealthIssueDTO(
            Guard.NotNull(healthIssueForm.ShortDescription, nameof(healthIssueForm.ShortDescription)),
            healthIssueForm.LongDescription
        );
    }
}