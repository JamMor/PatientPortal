#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

namespace PatientPortal.Services
{
    public class StaffViewService : IStaffViewService
    {
        private IStaffService _staffService;
        public StaffViewService(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public StaffInfoViewModel? GetStaffInfo(int staffId)
        {
            IQueryable<Staff> staffQuery = _staffService.GetStaffbyId(staffId);
            
            staffQuery = staffQuery
                .Include(staff => staff.Patients)
                .Include(staff => staff.MessagingLink);
            
            StaffInfoViewModel? staffInfo = staffQuery
            .Select(s => new StaffInfoViewModel()
            {
                StaffId = s.StaffId,
                MessagingLinkId = s.MessagingLink != null ? s.MessagingLink.MessagingLinkId : null,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Role = s.Role,
                PatientCount = s.Patients.Count,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .FirstOrDefault();

            return staffInfo;
        }

        public StaffManagerView ReturnStaffManagerView(StaffSearch searchQuery, Paginator displayProperties)
        {
            StaffManagerView managerView = new StaffManagerView()
            {
                SearchBar = searchQuery,
                PaginationSettings = displayProperties
            };

            var results = _staffService.SearchStaff(searchQuery);
            managerView.PaginationSettings.ResultsCount = results.Count();
            results = _staffService.SortStaff(results, displayProperties.SortOrder);

            //convert to DTOs and Paginate list
            managerView.SearchResults = results.Select(r => new StaffResult()
            {
                StaffId = r.StaffId,
                FullName = r.FullName(),
                Role = r.Role,
                PatientCount = r.Patients.Count,
                CreatedAt = r.CreatedAt
            })
            .ToPagedList(displayProperties.ResultsPerPage, displayProperties.CurrentPage);
            
            return managerView;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _staffService.Dispose();
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