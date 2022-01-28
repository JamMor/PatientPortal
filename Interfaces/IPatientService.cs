using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientService : IDisposable
    {
        Patient GetPatientbyId(int patientId);
        PatientManagerView GetPatientbyQuery(PatientSearch SearchBar, ListResultAttributes DisplayProperties);
        bool DoesPatientExist(PatientFormView patientInfo);
        int CreatePatient(PatientFormView patientInfo);
        void UpdatePatient(PatientFormView patientInfo);
        void DeletePatient(int patientId);
    }
}