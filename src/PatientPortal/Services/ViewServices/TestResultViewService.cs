using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

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

        public TestResultFormView? ReturnTestResultFormView(int patientId)
        {
            TestResultFormView? viewModel = _patientService.GetPatientBasicInfo()
                .Include(p => p.HealthIssues)
                .Select(p => new TestResultFormView()
                {
                    Patient = new PatientHeaderInfoView()
                    {
                        CurrentPatientId = p.PatientId,
                        CurrentPatientLinkId = 
                            p.MessagingLink != null ? p.MessagingLink.MessagingLinkId : null,
                        CurrentPatientFirstName = p.FirstName,
                        CurrentPatientLastName = p.LastName,
                        CurrentPatientSSN = p.Last4SSN,
                        CurrentPatientDOB = p.DOB,
                        CurrentPatientAge = p.Age,
                        CurrentPatientCreatedOn = p.CreatedAt
                    },
                    TestResultForm = new TestResultForm()
                    {
                        HealthIssues = p.HealthIssues
                            .Select(h => new HealthIssueCheckbox()
                                {
                                    HealthIssueId = h.HealthIssueId,
                                    ShortDescription = h.ShortDescription,
                                    CreatedAt = h.CreatedAt,
                                })
                            .ToList()
                    }
                })
                .FirstOrDefault(p => p.Patient.CurrentPatientId == patientId);

            return viewModel;
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