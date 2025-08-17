#nullable enable
using System;
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
    public static TestResultDTO ToTestResultDTO(this TestResultForm testResultForm)
    {
        var healthIssueIds = testResultForm.HealthIssues
            .Where(h => h.Selected)
            .Select(h => h.HealthIssueId)
            .ToList();

        return new TestResultDTO(
            Guard.NotNull(testResultForm.Type, nameof(testResultForm.Type)),
            Guard.NotNull(testResultForm.Comment, nameof(testResultForm.Comment)),
            healthIssueIds
        );
    }
}