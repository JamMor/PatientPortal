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

        public TestResultFormView GetNewTestResultForm(int patientId)
        {
            TestResultFormView form = new TestResultFormView();
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
    }
}