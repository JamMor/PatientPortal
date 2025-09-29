using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class VisitViewService : IVisitViewService
    {
        private IVisitService _visitService;
        private IPatientService _patientService;
        public VisitViewService(IVisitService visitService, IPatientService patientService)
        {
            _visitService = visitService;
            _patientService = patientService;
        }

        public VisitForm? ReturnVisitForm(int patientId)
        {
            VisitForm? form = _patientService.GetPatientBasicInfo()
                .Include(p => p.HealthIssues)
                .Where(p => p.PatientId == patientId)
                .Select(p => new VisitForm()
                {
                    HealthIssues = p.HealthIssues
                        .Select(h => new HealthIssueCheckbox()
                            {
                                HealthIssueId = h.HealthIssueId,
                                ShortDescription = h.ShortDescription,
                                CreatedAt = h.CreatedAt,
                            })
                        .ToList()
                })
                .FirstOrDefault();

            return form;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _patientService.Dispose();
                    _visitService.Dispose();
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