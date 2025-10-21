using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffViewService
    {
        StaffManagerView ReturnStaffManagerView(StaffFilter filter, Paginator paging, string sortOrder);
        StaffInfoViewModel? GetStaffInfo(int staffId);
    }
}