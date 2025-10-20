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
    }
}