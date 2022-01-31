using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
        public bool DoesPatientExist(PatientFormView patientInfo)
        {
            return _context.Patients.Any(patient =>
                    patient.Last4SSN == patientInfo.Last4SSN
                    && patient.DOB == patientInfo.DOB
                    && patient.FirstName == patientInfo.FirstName
                    && patient.LastName == patientInfo.LastName);
        }

        public int CreatePatient(PatientFormView patientInfo)
        {
            Patient newPatient = new Patient()
            {
                FirstName = patientInfo.FirstName,
                LastName = patientInfo.LastName,
                DOB = patientInfo.DOB,
                Last4SSN = patientInfo.Last4SSN,
                PhoneNumber = patientInfo.PhoneNumber,
                Email = patientInfo.Email,
                MessagingLink = new MessagingLink()
            };
            
            if(patientInfo.Address != null)
            {
                newPatient.Address = new Address()
                {
                    StreetAddress = patientInfo.Address.StreetAddress,
                    City = patientInfo.Address.City,
                    State = patientInfo.Address.State,
                    ZipCode = patientInfo.Address.ZipCode
                };
            }

            _context.Patients.Add(newPatient);
            _context.SaveChanges();

            return newPatient.PatientId;
        }

        public void DeletePatient(int patientId)
        {
            Patient deletedPatient = _context.Patients
                .Include(p => p.MessagingLink)
                .SingleOrDefault(patient => patient.PatientId == patientId);
            if (deletedPatient != null)
            {
                _context.Patients.Remove(deletedPatient);
                _context.SaveChanges();
            }
        }

        // QUERIES
        public Patient GetPatientbyId(int patientId)
        {
            Patient patient = _context.Patients
                .Include(patient => patient.HealthIssues
                    .OrderByDescending(h => h.UpdatedAt))
                .ThenInclude(issue => issue.AssociatedTestResults)
                .Include(patient => patient.HealthIssues)
                .ThenInclude(issue => issue.AssociatedVisits)
                .Include(patient => patient.Visits
                    .OrderByDescending(v => v.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.Tests
                    .OrderByDescending(t => t.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.MedicalTeam)
                .ThenInclude(team => team.Staff)
                .Include(p => p.MessagingLink)
                .FirstOrDefault(patient => patient.PatientId == patientId);

            return patient;
        }
        
        
        public IQueryable<Patient> SearchPatients(PatientSearch searchParams)
        {
            return _context.Patients
                .Where(patient => searchParams.SearchPatientId == null || patient.PatientId == searchParams.SearchPatientId)
                .Where(patient => string.IsNullOrEmpty(searchParams.SearchFirstName) || patient.FirstName.StartsWith(searchParams.SearchFirstName))
                .Where(patient => string.IsNullOrEmpty(searchParams.SearchLastName) || patient.LastName.StartsWith(searchParams.SearchLastName))
                .Where(patient => string.IsNullOrEmpty(searchParams.SearchSSN) || patient.Last4SSN == searchParams.SearchSSN)
                .Where(patient => searchParams.SearchBirthdate == null || patient.DOB == searchParams.SearchBirthdate);
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