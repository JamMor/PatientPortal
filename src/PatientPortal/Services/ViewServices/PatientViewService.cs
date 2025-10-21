using System.Linq;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class PatientViewService : IPatientViewService
    {
        private IPatientService _patientService;

        public PatientViewService(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public PatientHeaderInfoView? GetPatientInfoHeader(int patientId)
        {
            PatientHeaderInfoView? header = _patientService
                .GetPatientBasicInfo()
                .Select(p => new PatientHeaderInfoView()
                {
                    CurrentPatientId = p.PatientId,
                    CurrentPatientLinkId =
                        p.MessagingLink != null ? p.MessagingLink.MessagingLinkId : null,
                    CurrentPatientFirstName = p.FirstName,
                    CurrentPatientLastName = p.LastName,
                    CurrentPatientSSN = p.Last4SSN,
                    CurrentPatientDOB = p.DOB,
                    CurrentPatientAge = p.Age,
                    CurrentPatientCreatedOn = p.CreatedAt,
                })
                .FirstOrDefault(p => p.CurrentPatientId == patientId);

            return header;
        }

        public PatientInfoViewModel? GetPatientInfo(int patientId)
        {
            IQueryable<Patient> patientQuery = _patientService.GetPatientFullInfo();

            Patient? patient = patientQuery.FirstOrDefault(patient =>
                patient.PatientId == patientId
            );

            if (patient == null)
            {
                return null;
            }

            PatientInfoViewModel viewModel = new PatientInfoViewModel()
            {
                PatientHeader = new PatientHeaderInfoView()
                {
                    CurrentPatientId = patient.PatientId,
                    CurrentPatientLinkId =
                        patient.MessagingLink != null
                            ? patient.MessagingLink.MessagingLinkId
                            : null,
                    CurrentPatientFirstName = patient.FirstName,
                    CurrentPatientLastName = patient.LastName,
                    CurrentPatientSSN = patient.Last4SSN,
                    CurrentPatientDOB = patient.DOB,
                    CurrentPatientAge = patient.Age,
                    CurrentPatientCreatedOn = patient.CreatedAt,
                },
                PatientId = patient.PatientId,
                MessagingLinkId =
                    patient.MessagingLink != null 
                        ? patient.MessagingLink.MessagingLinkId 
                        : null,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DOB = patient.DOB,
                Last4SSN = patient.Last4SSN,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.Email,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt,
                Age = patient.Age,
                Address = patient.Address != null
                    ? new AddressInfo()
                    {
                        StreetAddress = patient.Address.StreetAddress,
                        City = patient.Address.City,
                        State = patient.Address.State,
                        ZipCode = patient.Address.ZipCode,
                    }
                    : null,
                MedicalTeam = patient
                    .MedicalTeam.Select(h => new StaffInfo()
                    {
                        StaffId = h.StaffId,
                        FullName = h.Staff?.FullName() ?? "Unknown Staff",
                        Role = h.Staff?.Role ?? "Unknown Role",
                    })
                    .ToList(),
                HealthIssues = patient
                    .HealthIssues.Select(h => new HealthIssueInfo()
                    {
                        HealthIssueId = h.HealthIssueId,
                        ShortDescription = h.ShortDescription,
                        LongDescription = h.LongDescription,
                        CreatedAt = h.CreatedAt,
                        UpdatedAt = h.UpdatedAt,
                        AssociatedVisitsCount = h.AssociatedVisits.Count,
                        AssociatedTestResultsCount = h.AssociatedTestResults.Count,
                    })
                    .OrderByDescending(h => h.CreatedAt)
                    .ToList(),
                Visits = patient
                    .Visits.Select(v => new VisitInfo()
                    {
                        VisitId = v.VisitId,
                        Comment = v.Comment,
                        DateOfVisit = v.DateOfVisit,
                        CreatedBy =
                            $"{v.Staff?.FullName() ?? "Unknown Staff"}, {v.Staff?.Role ?? "Unknown Role"}",
                        CreatedAt = v.CreatedAt,
                        UpdatedAt = v.UpdatedAt,
                    })
                    .OrderByDescending(v => v.CreatedAt)
                    .ToList(),
                TestResults = patient
                    .Tests.Select(t => new TestResultInfo()
                    {
                        TestResultId = t.TestResultId,
                        Type = t.Type,
                        Comment = t.Comment,
                        CreatedBy =
                            $"{t.Staff?.FullName() ?? "Unknown Staff"}, {t.Staff?.Role ?? "Unknown Role"}",
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                    })
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList(),
            };

            return viewModel;
        }

        public PatientManagerView ReturnPatientManagerView(
            PatientFilter filter,
            Paginator paging,
            string sortOrder,
            int staffId
        )
        {
            var managerView = new PatientManagerView
            {
                Query = PatientQuery.Create(filter, paging, sortOrder),
                Results = new PatientResultList(),
            };

            var results = _patientService.SearchPatients(filter, staffId);
            managerView.Query.Paging.ResultsCount = results.Count();
            results = _patientService.SortPatients(results, managerView.Query.Sort.SortString);

            managerView.Results.Patients = results
                .Select(r => new PatientResult
                {
                    PatientId = r.PatientId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    DOB = r.DOB,
                    Age = r.Age,
                    Last4SSN = r.Last4SSN,
                })
                .ToPagedList(paging.ResultsPerPage, paging.CurrentPage);

            return managerView;
        }
    }
}
