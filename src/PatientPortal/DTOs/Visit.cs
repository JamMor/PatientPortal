#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;
using PatientPortal.Shared.Guard;

namespace PatientPortal.DTOs;

public record VisitDTO(
    string Comment,
    DateTime DateOfVisit,
    List<int> HealthIssueIds
);

public static class VisitFormDTOExtensions
{
    public static VisitDTO ToVisitDTO(this VisitForm visitForm)
    {
        var healthIssueIds = visitForm.HealthIssues
            .Where(h => h.Selected)
            .Select(h => h.HealthIssueId)
            .ToList();
            
        return new VisitDTO(
            Guard.NotNull(visitForm.Comment, nameof(visitForm.Comment)),
            visitForm.DateOfVisit,
            healthIssueIds
        );
    }
}