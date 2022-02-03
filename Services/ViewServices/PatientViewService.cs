using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

namespace PatientPortal.Services
{
    public class PatientViewService : IPatientViewService
    {
        private IPatientService _patientService;
        public PatientViewService(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public PatientInfoViewModel GetPatientInfo(int patientId)
        {
            IQueryable<Patient> patientQuery = _patientService.GetPatientbyId(patientId);

            PatientInfoViewModel patientInfo = patientQuery
                .Select(p => new PatientInfoViewModel()
                {
                    PatientId = p.PatientId,
                    MessagingLinkId = p.MessagingLink.MessagingLinkId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    DOB = p.DOB,
                    Last4SSN = p.Last4SSN,
                    PhoneNumber = p.PhoneNumber,
                    Email = p.Email,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Age = p.Age,
                    Address = new AddressInfo()
                    {
                        StreetAddress = p.Address.StreetAddress,
                        City = p.Address.City,
                        State = p.Address.State,
                        ZipCode = p.Address.ZipCode,
                    },
                    MedicalTeam = p.MedicalTeam
                        .Select(h => new StaffInfo()
                        {
                            StaffId = h.StaffId,
                            FullName = h.Staff.FullName(),
                            Role = h.Staff.Role
                        })
                        .ToList(),
                    HealthIssues = p.HealthIssues
                        .Select(h => new HealthIssueInfo()
                        {
                            HealthIssueId = h.HealthIssueId,
                            ShortDescription = h.ShortDescription,
                            LongDescription = h.LongDescription,
                            CreatedAt = h.CreatedAt,
                            UpdatedAt = h.UpdatedAt,
                            AssociatedVisitsCount = h.AssociatedVisits.Count,
                            AssociatedTestResultsCount = h.AssociatedTestResults.Count
                        })
                        .OrderByDescending(h => h.CreatedAt)
                        .ToList(),
                    Visits = p.Visits
                        .Select(v => new VisitInfo()
                        {
                            VisitId = v.VisitId,
                            Comment = v.Comment,
                            DateOfVisit = v.DateOfVisit,
                            CreatedBy = $"{v.Staff.FullName()}, {v.Staff.Role}",
                            CreatedAt = v.CreatedAt,
                            UpdatedAt = v.UpdatedAt
                        })
                        .OrderByDescending(v => v.CreatedAt)
                        .ToList(),
                    TestResults = p.Tests
                        .Select(t => new TestResultInfo()
                        {
                            TestResultId = t.TestResultId,
                            Type = t.Type,
                            Comment = t.Comment,
                            CreatedBy = $"{t.Staff.FullName()}, {t.Staff.Role}",
                            CreatedAt = t.CreatedAt,
                            UpdatedAt = t.UpdatedAt
                        })
                        .OrderByDescending(t => t.CreatedAt)
                        .ToList()
                })
                .FirstOrDefault(patient => patient.PatientId == patientId);

            return patientInfo;
        }

        public PatientManagerView ReturnPatientManagerView(PatientSearch searchQuery, ListResultAttributes displayProperties)
        {
            PatientManagerView managerView = new PatientManagerView()
            {
                SearchBar = searchQuery,
                DisplayProperties = displayProperties
            };

            var results = _patientService.SearchPatients(searchQuery);
            managerView.DisplayProperties.ResultsCount = results.Count();
            results = _patientService.SortPatients(results, displayProperties.SortOrder);

            //convert to DTOs and Paginate list
            managerView.SearchResults = results.Select(r => new PatientResult()
            {
                PatientId = r.PatientId,
                FirstName = r.FirstName,
                LastName = r.LastName,
                DOB = r.DOB,
                Age = r.Age,
                Last4SSN = r.Last4SSN
            })
            .ToPagedList(displayProperties.ResultsPerPage, displayProperties.CurrentPage);
            
            return managerView;
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