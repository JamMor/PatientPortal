using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffService : IDisposable
    {
        //Commands
        bool DoesStaffExist(string staffUsername);
        int CreateStaff(StaffFormView staffInfo);
        // void UpdateStaff(StaffFormView staffInfo);
        void DeleteStaff(int staffId);

        //Queries
        Staff GetStaffbyId(int staffId);
        StaffManagerView GetStaffbyQuery(StaffSearch SearchBar, ListResultAttributes DisplayProperties);
    }
}