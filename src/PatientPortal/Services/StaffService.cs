#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.DTOs;

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
       
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}