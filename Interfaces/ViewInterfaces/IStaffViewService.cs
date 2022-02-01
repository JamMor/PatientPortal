using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffViewService : IDisposable
    {
        StaffManagerView ReturnStaffManagerView(StaffSearch searchQuery, ListResultAttributes displayProperties);
        StaffInfoViewModel GetStaffInfo(int staffId);
    }
}