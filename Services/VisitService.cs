using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class VisitService : IVisitService
    {
        private PatientPortalContext _context;
        public VisitService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public void CreateVisit(int patientId, int staffId, VisitFormView formData)
        {
            Visit newVisit = new Visit()
            {
                Comment = formData.Visit.Comment,
                DateOfVisit = formData.Visit.DateOfVisit,
                PatientId = patientId,
                StaffId = staffId,
                AssociatedHealthIssues = formData.HealthIssues
                    .Where(h => h.Selected == true)
                    .Select(h => new VisitHealthIssueAssociation()
                    {
                        HealthIssueId = h.HealthIssueId
                    })
                    .ToList()
            };
            
            _context.Visits.Add(newVisit);
            _context.SaveChanges();
        }
        public void DeleteVisit(int visitId)
        {
            Visit deletedVisit = _context.Visits
                .SingleOrDefault(visit => visit.VisitId == visitId);

            if(deletedVisit != null)
            {
                _context.Visits.Remove(deletedVisit);
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