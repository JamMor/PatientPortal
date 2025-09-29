using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class TestResultViewService : ITestResultViewService
    {
        private ITestResultService _testResultService;
        private IPatientService _patientService;
        public TestResultViewService(ITestResultService testResultService, IPatientService patientService)
        {
            _testResultService = testResultService;
            _patientService = patientService;
        }

        public TestResultForm? ReturnTestResultForm(int patientId)
        {
            TestResultForm? form = _patientService.GetPatientBasicInfo()
                .Include(p => p.HealthIssues)
                .Where(p => p.PatientId == patientId)
                .Select(p => new TestResultForm()
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
                    _testResultService.Dispose();
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