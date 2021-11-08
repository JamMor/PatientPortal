using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/provider/staff")]
    public class StaffController : Controller
    {
        private int? uuid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }
        private bool IsLoggedIn
        {
            get
            {
                return uuid != null;
            }
        }

        private PatientPortalContext _context;
        public StaffController(PatientPortalContext context)
        {
            _context = context;
        }

//==============Staff Manager==============================
        [HttpGet("")]
        public IActionResult StaffManager()
        {
            StaffSearch EmptySearch = new StaffSearch();

            List<Staff> AllResults = _context.Staff
                .Include(staff => staff.Patients)
                .ToList();

            StaffManagerView ViewModel = new StaffManagerView
            {
                SearchBar = EmptySearch,
                SearchResults = AllResults
            };

            return View("StaffManager", ViewModel);
        }

        [HttpPost("")]
        public IActionResult StaffManagerQuery(StaffSearch StaffQuery)
        {
            List<Staff> QueryResults = _context.Staff
                .Where(staff => StaffQuery.SearchStaffId == null || staff.StaffId == StaffQuery.SearchStaffId)
                .Where(staff => string.IsNullOrEmpty(StaffQuery.SearchFirstName) || staff.FirstName.StartsWith(StaffQuery.SearchFirstName))
                .Where(staff => string.IsNullOrEmpty(StaffQuery.SearchLastName) || staff.LastName.StartsWith(StaffQuery.SearchLastName) )
                .Where(staff => string.IsNullOrEmpty(StaffQuery.SearchRole) || staff.Role == StaffQuery.SearchRole)
                .Include(staff => staff.Patients)
                .ToList();

            StaffManagerView ViewModel = new StaffManagerView
            {
                SearchBar = StaffQuery,
                SearchResults = QueryResults
            };

            return View("StaffManager", ViewModel);
        }

        [HttpGet("add")]
        public IActionResult StaffAdd()
        {
            return View("StaffForm");
        }

        [HttpPost("add")]
        public IActionResult StaffCreate(Staff newStaff)
        {
            if(!_context.Staff.Any(staff => staff.StaffUsername == newStaff.StaffUsername))
            {
                if(ModelState.IsValid)
                {
                    PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
                    newStaff.Password = hasher.HashPassword(newStaff, newStaff.Password);
                    newStaff.IsAdmin = false;

                    _context.Staff.Add(newStaff);
                    _context.SaveChanges();

                    // MessagingLink newLink = new MessagingLink()
                    // {
                    //     StaffId = newStaff.StaffId
                    // };

                    // _context.MessagingLinks.Add(newLink);
                    // _context.SaveChanges();

                    return RedirectToAction("StaffManager", "Staff");
                }
            }
            else
            {
                ModelState.AddModelError("StaffUsername","This username is already taken.");
            }
            return View("StaffForm");
        }

        [HttpGet("{staffId}")]
        public IActionResult StaffInfo(int staffId)
        {
            Staff staffmember = _context.Staff
                .Include(staff => staff.Patients)
                .FirstOrDefault(staff => staff.StaffId == staffId);
            return View("StaffInfo", staffmember);
        }
    

        [HttpPost("{staffId}/delete")]
        public IActionResult StaffDelete(int staffId)
        {
            Staff deletedStaff = _context.Staff.SingleOrDefault(staff => staff.StaffId == staffId);
            if(deletedStaff != null)
            {
                _context.Staff.Remove(deletedStaff);
                _context.SaveChanges();
            }
            return RedirectToAction("StaffManager", "Staff");
        }

    }
}