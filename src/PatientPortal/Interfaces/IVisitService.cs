using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface IVisitService
    {

        void CreateVisit(int patientId, int staffId, VisitDTO formData);
        void DeleteVisit(int visitId);
    }
}