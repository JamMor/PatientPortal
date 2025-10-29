using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestResultViewService
    {
        TestResultFormView GetNewTestResultForm(int patientId);
    }
}