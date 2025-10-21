using System.Linq;
using PatientPortal.DTOs;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientService
    {
        bool DoesPatientExist(PatientDTO patientInfo);
        int CreatePatient(PatientDTO patientInfo);
        void DeletePatient(int patientId);

        IQueryable<Patient> GetPatientBasicInfo();
        IQueryable<Patient> GetPatientFullInfo();
        IQueryable<Patient> SearchPatients(PatientFilter searchParams, int staffId);
        IQueryable<Patient> SortPatients(IQueryable<Patient> query, string sortOrder);
    }
}