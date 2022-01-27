using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientService : IDisposable
    {
        // Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId);
        // Task<IEnumerable<SelectListItem>> GetBrands();
        // Task<IEnumerable<SelectListItem>> GetTypes();

        // IEnumerable<Student> GetStudents();
        // Student GetStudentByID(int studentId);
        // void InsertStudent(Student student);
        // void DeleteStudent(int studentID);
        // void UpdateStudent(Student student);
        // void Save();

        Patient GetPatientbyId(int patientId);
        List<Patient> GetPatientbyQuery(PatientSearch searchQuery);
        void CreatePatient(PatientFormView patientInfo);
        void UpdatePatient(PatientFormView patientInfo);
        void DeletePatient(int patientId);

        void AddStaffToPatientTeam(int patientId, int staffId);
        void RemoveStaffFromPatientTeam(int patientId, int staffId);

        void Save();
    }
}