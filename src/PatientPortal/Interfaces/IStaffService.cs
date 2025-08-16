#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using PatientPortal.Models;
using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface IStaffService : IDisposable
    {
        //Commands
        Staff CreateStaff(StaffDTO staffDTO, IdentityUser user);
        void DeleteStaff(int staffId);

        //Queries
        IQueryable<Staff> GetStaffbyId(int staffId);
        IQueryable<Staff> SearchStaff(StaffSearch searchParams);
        IQueryable<Staff> SortStaff(IQueryable<Staff> query, string sortOrder);
    }
}