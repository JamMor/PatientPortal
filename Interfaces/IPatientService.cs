using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientService : IDisposable
    {
        bool DoesPatientExist(PatientFormView patientInfo);
        int CreatePatient(PatientFormView patientInfo);
        void DeletePatient(int patientId);

        IQueryable<Patient> GetPatientBasicInfo();
        IQueryable<Patient> GetPatientFullInfo();
        IQueryable<Patient> SearchPatients(PatientSearch searchParams);
        IQueryable<Patient> SortPatients(IQueryable<Patient> query, string sortOrder);
    }
}