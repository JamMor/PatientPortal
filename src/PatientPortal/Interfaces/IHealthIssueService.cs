using System;
using System.Collections.Generic;
using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface IHealthIssueService : IDisposable
    {
        void CreateHealthIssue(int patientId, HealthIssueDTO healthIssueInfo);
        void DeleteHealthIssue(int patientId, int healthIssueId);
    }
}