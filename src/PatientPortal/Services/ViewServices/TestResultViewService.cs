using System.Linq;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class TestResultViewService : ITestResultViewService
    {
        private ITestResultService _testResultService;
        private IHealthIssueService _healthIssueService;
        public TestResultViewService(ITestResultService testResultService, IHealthIssueService healthIssueService)
        {
            _testResultService = testResultService;
            _healthIssueService = healthIssueService;
        }

        public TestResultForm GetNewTestResultForm(int patientId)
        {
            TestResultForm form = new TestResultForm();
            var healthIssues = _healthIssueService
                .GetHealthIssuesByPatientId(patientId)
                .Select(h => new HealthIssueCheckbox()
                {
                    HealthIssueId = h.HealthIssueId,
                    ShortDescription = h.ShortDescription,
                    CreatedAt = h.CreatedAt,
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