using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientViewService
    {
        PatientHeaderInfoView? GetPatientInfoHeader(int patientId);
        PatientInfoViewModel? GetPatientInfo(int patientId);
        PatientManagerView ReturnPatientManagerView(PatientSearch searchQuery, Paginator paginationSettings, int staffId);
    }
}