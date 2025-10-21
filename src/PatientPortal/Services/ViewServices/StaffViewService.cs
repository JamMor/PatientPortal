using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

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
                    MessagingLinkId = s.MessagingLink != null 
                        ? s.MessagingLink.MessagingLinkId 
                        : null,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Role = s.Role,
                    PatientCount = s.Patients.Count,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                })
                .FirstOrDefault();

            return staffInfo;
        }

        public StaffManagerView ReturnStaffManagerView(
            StaffFilter filter,
            Paginator paging,
            string sortOrder
        )
        {
            var managerView = new StaffManagerView
            {
                Query = StaffQuery.Create(filter, paging, sortOrder),
                Results = new StaffResultList(),
            };

            var results = _staffService.SearchStaff(filter);
            managerView.Query.Paging.ResultsCount = results.Count();
            results = _staffService.SortStaff(results, managerView.Query.Sort.SortString);

            managerView.Results.Staff = results
                .Select(r => new StaffResult
                {
                    StaffId = r.StaffId,
                    FullName = r.FullName(),
                    Role = r.Role,
                    PatientCount = r.Patients.Count,
                    CreatedAt = r.CreatedAt,
                })
                .ToPagedList(paging.ResultsPerPage, paging.CurrentPage);

            return managerView;
        }
    }
}
