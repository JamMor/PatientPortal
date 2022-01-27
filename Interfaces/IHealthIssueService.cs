using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IHealthIssueService : IDisposable
    {

        HealthIssue GetHealthIssuebyId(int healthIssueId);
        void CreateHealthIssue(HealthIssue healthIssueInfo);
        void UpdateHealthIssue(HealthIssue healthIssueInfo);
        void DeleteHealthIssue(int patientId, int healthIssueId);

        void Save();
    }
}