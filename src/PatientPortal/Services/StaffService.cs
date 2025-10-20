using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class StaffService : IStaffService
    {
        private PatientPortalContext _context;
        public StaffService(PatientPortalContext context)
        {
            _context = context;
        }

        //COMMANDS
        // Creates NON-admin staff, with auth
        public Staff CreateStaff(StaffDTO staffDTO, IdentityUser user)
        {
            Staff newStaff = new Staff()
            {
                FirstName = staffDTO.FirstName,
                LastName = staffDTO.LastName,
                Role = staffDTO.Role,
                User = user,
                IsAdmin = false,
                MessagingLink = new MessagingLink(),
                // TODO: Remove these legacy fields after migration
                StaffUsername = user.UserName!,
                Password = "[Managed by Identity]"
            };

            _context.Staff.Add(newStaff);
            _context.SaveChanges();

            return newStaff;
        }

        public void DeleteStaff(int staffId)
        {
            Staff? deletedStaff = _context.Staff
                .Include(s => s.MessagingLink)
                .SingleOrDefault(staff => staff.StaffId == staffId);
            if(deletedStaff != null)
            {
                _context.Staff.Remove(deletedStaff);
                _context.SaveChanges();
            }
        }

        // QUERIES
        public IQueryable<Staff> GetStaffbyId(int staffId)
        {
            IQueryable<Staff> staffmember = _context.Staff
                .Where(staff => staff.StaffId == staffId);

            return staffmember;
        }
        
        public IQueryable<Staff> SearchStaff(StaffSearch searchParams)
        {
            return _context.Staff
                .Where(staff => searchParams.SearchStaffId == null || staff.StaffId == searchParams.SearchStaffId)
                .Where(staff => string.IsNullOrEmpty(searchParams.SearchFirstName) || staff.FirstName.StartsWith(searchParams.SearchFirstName))
                .Where(staff => string.IsNullOrEmpty(searchParams.SearchLastName) || staff.LastName.StartsWith(searchParams.SearchLastName) )
                .Where(staff => string.IsNullOrEmpty(searchParams.SearchRole) || staff.Role == searchParams.SearchRole);
        }

        public IQueryable<Staff> SortStaff(IQueryable<Staff> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "StaffId_desc":
                    query = query.OrderByDescending(s => s.StaffId);
                    break;
                case "StaffId_asc":
                    query = query.OrderBy(s => s.StaffId);
                    break;
                case "LastName_desc":
                    query = query.OrderByDescending(s => s.LastName);
                    break;
                case "LastName_asc":
                    query = query.OrderBy(s => s.LastName);
                    break;
                case "Role_desc":
                    query = query.OrderByDescending(s => s.Role);
                    break;
                case "Role_asc":
                    query = query.OrderBy(s => s.Role);
                    break;
                default:
                    query = query.OrderBy(s => s.LastName);
                    break;
            }
            return query;
        }
    }
}