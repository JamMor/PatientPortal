using System;
using System.Linq;
using PatientPortal.DTOs;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IHealthIssueService : IDisposable
    {
        void CreateHealthIssue(int patientId, HealthIssueDTO healthIssueInfo);
        void DeleteHealthIssue(int patientId, int healthIssueId);
        IQueryable<HealthIssue> GetHealthIssuesByPatientId(int patientId);
    }
}