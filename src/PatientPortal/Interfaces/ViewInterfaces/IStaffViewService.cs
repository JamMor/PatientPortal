using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffViewService : IDisposable
    {
        StaffManagerView ReturnStaffManagerView(StaffSearch searchQuery, Paginator paginationSettings);
        StaffInfoViewModel? GetStaffInfo(int staffId);
    }
}