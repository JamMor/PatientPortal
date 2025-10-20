using System.Linq;
using PatientPortal.DTOs;
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
        public void CreateHealthIssue(int patientId, HealthIssueDTO healthIssueInfo)
        {
            var healthIssue = new HealthIssue
            {
                ShortDescription = healthIssueInfo.ShortDescription,
                LongDescription = healthIssueInfo.LongDescription,
                PatientId = patientId
            };
            _context.HealthIssues.Add(healthIssue);
            _context.SaveChanges();
        }
        public void DeleteHealthIssue(int patientId, int issueId)
        {
            HealthIssue? deletedHealthIssue = _context.HealthIssues
                .SingleOrDefault(issue => issue.HealthIssueId == issueId);
            if(deletedHealthIssue != null)
            {
                _context.HealthIssues.Remove(deletedHealthIssue);
                _context.SaveChanges();
            }
        }

        //QUERIES
        public IQueryable<HealthIssue> GetHealthIssuesByPatientId(int patientId)
        {
            return _context.HealthIssues
                .Where(issue => issue.PatientId == patientId);
        }
    }
}