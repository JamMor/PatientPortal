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
        public IActionResult StaffManager(StaffSearch SearchBar, ListResultAttributes DisplayProperties)
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