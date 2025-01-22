using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffService : IDisposable
    {
        //Commands
        bool DoesStaffExist(string staffUsername);
        int CreateStaff(StaffFormView staffInfo);
        void DeleteStaff(int staffId);

        //Queries
        IQueryable<Staff> GetStaffbyId(int staffId);
        IQueryable<Staff> SearchStaff(StaffSearch searchParams);
        IQueryable<Staff> SortStaff(IQueryable<Staff> query, string sortOrder);
    }
}