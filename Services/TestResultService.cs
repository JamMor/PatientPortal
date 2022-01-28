using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
        public void CreateTestResult(int patientId, int staffId, TestResultFormView formData)
        {
            TestResult testData = new TestResult()
            {
                Type = formData.TestResult.Type,
                Comment = formData.TestResult.Comment,
                PatientId = patientId,
                StaffId = staffId,
                AssociatedHealthIssues = formData.HealthIssues
                .Where(h => h.Selected == true)
                .Select(h => new TestHealthIssueAssociation()
                {
                    HealthIssueId = h.HealthIssueId
                })
                .ToList()
            };

            _context.TestResults.Add(testData);
            _context.SaveChanges();
            // foreach (int issueId in issues)
            // {
            //     TestHealthIssueAssociation newAssociation = new TestHealthIssueAssociation()
            //     {
            //         TestResultId = testData.TestResultId,
            //         HealthIssueId = issueId
            //     };
            //     _context.TestHealthIssueAssociations.Add(newAssociation);
            //     _context.SaveChanges();
            // }
        }
        public void DeleteTestResult(int testResultId)
        {
            TestResult deletedTestResult = _context.TestResults
                .SingleOrDefault(issue => issue.TestResultId == testResultId);
            if(deletedTestResult != null)
            {
                _context.TestResults.Remove(deletedTestResult);
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