using System.Linq;
using PatientPortal.DTOs;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class TestResultService : ITestResultService
    {
        private PatientPortalContext _context;
        public TestResultService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public void CreateTestResult(int patientId, int staffId, TestResultDTO formData)
        {
            TestResult newTestResult = new TestResult()
            {
                Type = formData.Type,
                Comment = formData.Comment,
                PatientId = patientId,
                StaffId = staffId,
                AssociatedHealthIssues = formData.HealthIssueIds
                    .Select(h => new TestHealthIssueAssociation()
                    {
                        HealthIssueId = h
                    })
                    .ToList()
            };

            _context.TestResults.Add(newTestResult);
            _context.SaveChanges();
        }
        public void DeleteTestResult(int testResultId)
        {
            TestResult? deletedTestResult = _context.TestResults
                .SingleOrDefault(t => t.TestResultId == testResultId);
            if(deletedTestResult != null)
            {
                _context.TestResults.Remove(deletedTestResult);
                _context.SaveChanges();
            }
        }
    }
}