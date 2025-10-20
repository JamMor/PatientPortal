using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffViewService
    {
        StaffManagerView ReturnStaffManagerView(StaffSearch searchQuery, Paginator paginationSettings);
        StaffInfoViewModel? GetStaffInfo(int staffId);
    }
}