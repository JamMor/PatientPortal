using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitViewService
    {
        VisitFormView GetNewVisitForm(int patientId);
    }
}