using System.Linq;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class VisitViewService : IVisitViewService
    {
        private IVisitService _visitService;
        private IHealthIssueService _healthIssueService;
        public VisitViewService(IVisitService visitService, IHealthIssueService healthIssueService)
        {
            _visitService = visitService;
            _healthIssueService = healthIssueService;
        }

        public VisitFormView GetNewVisitForm(int patientId)
        {
            VisitFormView form = new VisitFormView();
            var healthIssues = _healthIssueService
                .GetHealthIssuesByPatientId(patientId)
                .Select(h => new HealthIssueCheckbox()
                {
                    HealthIssueId = h.HealthIssueId,
                    ShortDescription = h.ShortDescription,
                })
                .ToList();

            form.HealthIssues = healthIssues;

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
                    _healthIssueService.Dispose();
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