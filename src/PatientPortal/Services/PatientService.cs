using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class PatientService : IPatientService
    {
        private PatientPortalContext _context;

        public PatientService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        public bool DoesPatientExist(PatientDTO patientInfo)
        {
            return _context.Patients.Any(patient =>
                patient.Last4SSN == patientInfo.Last4SSN
                && patient.DOB == patientInfo.DOB
                && patient.FirstName == patientInfo.FirstName
                && patient.LastName == patientInfo.LastName
            );
        }

        public int CreatePatient(PatientDTO patientInfo)
        {
            Patient newPatient = new Patient()
            {
                FirstName = patientInfo.FirstName,
                LastName = patientInfo.LastName,
                DOB = patientInfo.DOB,
                Last4SSN = patientInfo.Last4SSN,
                PhoneNumber = patientInfo.PhoneNumber,
                Email = patientInfo.Email,
                MessagingLink = new MessagingLink(),
            };

            if (patientInfo.Address != null)
            {
                newPatient.Address = new Address()
                {
                    StreetAddress = patientInfo.Address.StreetAddress,
                    City = patientInfo.Address.City,
                    State = patientInfo.Address.State,
                    ZipCode = patientInfo.Address.ZipCode,
                };
            }

            _context.Patients.Add(newPatient);
            _context.SaveChanges();

            return newPatient.PatientId;
        }

        public void DeletePatient(int patientId)
        {
            Patient? deletedPatient = _context.Patients
                .Include(p => p.MessagingLink)
                .SingleOrDefault(patient => patient.PatientId == patientId);
            if (deletedPatient != null)
            {
                _context.Patients.Remove(deletedPatient);
                _context.SaveChanges();
            }
        }

        // QUERIES
        public IQueryable<Patient> GetPatientBasicInfo()
        {
            IQueryable<Patient> patient = _context.Patients
                .Include(p => p.MessagingLink);

            return patient;
        }

        public IQueryable<Patient> GetPatientFullInfo()
        {
            IQueryable<Patient> patient = _context.Patients
                .Include(patient => patient.HealthIssues)
                    .ThenInclude(issue => issue.AssociatedTestResults)
                .Include(patient => patient.HealthIssues)
                    .ThenInclude(issue => issue.AssociatedVisits)
                .Include(patient => patient.Visits)
                    .ThenInclude(team => team.Staff)
                .Include(patient => patient.Tests)
                    .ThenInclude(team => team.Staff)
                .Include(patient => patient.MedicalTeam)
                    .ThenInclude(team => team.Staff)
                .Include(p => p.MessagingLink)
                .AsSplitQuery();

            return patient;
        }

        public IQueryable<Patient> SearchPatients(PatientFilter searchParams, int staffId)
        {
            var results = _context.Patients
                .Where(patient => searchParams.PatientId == null || patient.PatientId == searchParams.PatientId)
                .Where(patient => string.IsNullOrEmpty(searchParams.FirstName) || patient.FirstName.StartsWith(searchParams.FirstName))
                .Where(patient => string.IsNullOrEmpty(searchParams.LastName) || patient.LastName.StartsWith(searchParams.LastName))
                .Where(patient => string.IsNullOrEmpty(searchParams.SSN) || patient.Last4SSN == searchParams.SSN)
                .Where(patient => searchParams.Birthdate == null || patient.DOB.Date == searchParams.Birthdate.Value.Date);

            if (searchParams.OnlyPatientsUnderCare == true)
            {
                results = results
                    .Include(patient => patient.MedicalTeam)
                    .Where(patient => patient.MedicalTeam.Any(m => m.StaffId == staffId));
            }

            return results;
        }

        public IQueryable<Patient> SortPatients(IQueryable<Patient> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "PatientId_desc":
                    query = query.OrderByDescending(p => p.PatientId);
                    break;
                case "PatientId_asc":
                    query = query.OrderBy(p => p.PatientId);
                    break;
                case "LastName_desc":
                    query = query.OrderByDescending(p => p.LastName);
                    break;
                case "LastName_asc":
                    query = query.OrderBy(p => p.LastName);
                    break;
                case "DOB_desc":
                    query = query.OrderByDescending(p => p.DOB);
                    break;
                case "DOB_asc":
                    query = query.OrderBy(p => p.DOB);
                    break;
                default:
                    query = query.OrderBy(s => s.LastName);
                    break;
            }

            return query;
        }
    }
}
