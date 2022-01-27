using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffService : IDisposable
    {

        Staff GetStaffbyId(int staffId);
        List<Staff> GetStaffbyQuery(StaffSearch searchQuery);
        void CreateStaff(StaffFormView staffInfo);
        void UpdateStaff(StaffFormView staffInfo);
        void DeleteStaff(int staffId);

        void Save();
    }
}