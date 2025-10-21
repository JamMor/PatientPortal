using System.Linq;
using Microsoft.AspNetCore.Identity;
using PatientPortal.DTOs;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffService
    {
        //Commands
        Staff CreateStaff(StaffDTO staffDTO, IdentityUser user);
        void DeleteStaff(int staffId);

        //Queries
        IQueryable<Staff> GetStaffbyId(int staffId);
        IQueryable<Staff> SearchStaff(StaffFilter searchParams);
        IQueryable<Staff> SortStaff(IQueryable<Staff> query, string sortOrder);
    }
}