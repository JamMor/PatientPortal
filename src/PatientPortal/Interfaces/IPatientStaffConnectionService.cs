namespace PatientPortal.Interfaces
{
    public interface IPatientStaffConnectionService
    {
        void AddStaffToPatientTeam(int patientId, int staffId);
        void RemoveStaffFromPatientTeam(int patientId, int staffId);
    }
}