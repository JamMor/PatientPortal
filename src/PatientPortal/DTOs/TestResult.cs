using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record TestResultDTO(
    string Type,
    string Comment,
    List<int> HealthIssueIds
);

public static class TestResultFormDTOExtensions
{
    public static TestResultDTO ToTestResultDTO(this TestResultFormInput input)
    {
        var healthIssueIds = input.HealthIssues
            .Where(h => h.Selected)
            .Select(h => h.HealthIssueId)
            .ToList();

        return new TestResultDTO(
            Guard.NotNull(input.Type, nameof(input.Type)),
            Guard.NotNull(input.Comment, nameof(input.Comment)),
            healthIssueIds
        );
    }
}