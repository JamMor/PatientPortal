using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public bool DoesStaffExist(string staffUsername)
        {
            return _context.Staff
                .Any(staff => staff.StaffUsername == staffUsername);
        }

        public int CreateStaff(StaffFormView staffFormView)
        {
            Staff newStaff = new Staff()
            {
                FirstName = staffFormView.FirstName,
                LastName = staffFormView.LastName,
                Role = staffFormView.Role,
                StaffUsername = staffFormView.StaffUsername,
                Password = staffFormView.Password,
                IsAdmin = false,
                MessagingLink = new MessagingLink()
            };

            PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
            newStaff.Password = hasher.HashPassword(newStaff, newStaff.Password);
            
            _context.Staff.Add(newStaff);
            _context.SaveChanges();

            return newStaff.StaffId;
        }

        public void DeleteStaff(int staffId)
        {
            Staff deletedStaff = _context.Staff
                .Include(s => s.MessagingLink)
                .SingleOrDefault(staff => staff.StaffId == staffId);
            if(deletedStaff != null)
            {
                _context.Staff.Remove(deletedStaff);
                _context.SaveChanges();
            }
        }

        // QUERIES
        public Staff GetStaffbyId(int staffId)
        {
            Staff staffmember = _context.Staff
                .Include(staff => staff.Patients)
                .Include(staff => staff.MessagingLink)
                .FirstOrDefault(staff => staff.StaffId == staffId);

            return staffmember;
        }
        public StaffManagerView GetStaffbyQuery(StaffSearch SearchBar, ListResultAttributes DisplayProperties)
        
        {            
            var staffQuery = _context.Staff
                .Where(staff => SearchBar.SearchStaffId == null || staff.StaffId == SearchBar.SearchStaffId)
                .Where(staff => string.IsNullOrEmpty(SearchBar.SearchFirstName) || staff.FirstName.StartsWith(SearchBar.SearchFirstName))
                .Where(staff => string.IsNullOrEmpty(SearchBar.SearchLastName) || staff.LastName.StartsWith(SearchBar.SearchLastName) )
                .Where(staff => string.IsNullOrEmpty(SearchBar.SearchRole) || staff.Role == SearchBar.SearchRole);

            switch (DisplayProperties.SortOrder)
            {
                case "StaffId_desc":
                    staffQuery = staffQuery.OrderByDescending(s => s.StaffId);
                    break;
                case "StaffId_asc":
                    staffQuery = staffQuery.OrderBy(s => s.StaffId);
                    break;
                case "LastName_desc":
                    staffQuery = staffQuery.OrderByDescending(s => s.LastName);
                    break;
                case "LastName_asc":
                    staffQuery = staffQuery.OrderBy(s => s.LastName);
                    break;
                case "Role_desc":
                    staffQuery = staffQuery.OrderByDescending(s => s.Role);
                    break;
                case "Role_asc":
                    staffQuery = staffQuery.OrderBy(s => s.Role);
                    break;
                default:
                    staffQuery = staffQuery.OrderBy(s => s.LastName);
                    break;
            }

            DisplayProperties.ResultsCount = staffQuery.Count();

            List<Staff> queryResults = staffQuery
                .Include(staff => staff.Patients)
                .Skip(DisplayProperties.ResultsPerPage*(DisplayProperties.CurrentPage-1))
                .Take(DisplayProperties.ResultsPerPage)
                .ToList();

            StaffManagerView ViewModel = new StaffManagerView
            {
                SearchBar = SearchBar,
                SearchResults = queryResults,
                DisplayProperties = DisplayProperties
            };

            return ViewModel;
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