using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class HealthIssueService : IHealthIssueService
    {
        private PatientPortalContext _context;
        public HealthIssueService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public void CreateHealthIssue(int patientId, HealthIssue healthIssueInfo)
        {
            healthIssueInfo.PatientId = patientId;
            _context.HealthIssues.Add(healthIssueInfo);
            _context.SaveChanges();
        }
        public void DeleteHealthIssue(int patientId, int issueId)
        {
            HealthIssue deletedHealthIssue = _context.HealthIssues
                .SingleOrDefault(issue => issue.HealthIssueId == issueId);
            if(deletedHealthIssue != null)
            {
                _context.HealthIssues.Remove(deletedHealthIssue);
                _context.SaveChanges();
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}