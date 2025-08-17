#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

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

        public VisitFormView? ReturnVisitFormView(int patientId)
        {
            VisitFormView? viewModel = _patientService.GetPatientBasicInfo()
                .Include(p => p.HealthIssues)
                .Select(p => new VisitFormView()
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
                    VisitForm = new VisitForm()
                    {
                        HealthIssues = p.HealthIssues
                            .Select(h => new HealthIssueCheckbox()
                                {
                                    HealthIssueId = h.HealthIssueId,
                                    ShortDescription = h.ShortDescription,
                                    CreatedAt = h.CreatedAt,
                                })
                            .ToList()
                    },
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