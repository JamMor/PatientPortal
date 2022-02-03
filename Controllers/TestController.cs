using System;
using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Interfaces;

namespace PatientPortal.Controllers
{
    
    public class TestController : Controller
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
        private ISeedViewService _seedViewService;

        public TestController(
            PatientPortalContext context, 
            ISeedViewService seedViewService
            )
        {
            _context = context;
            _seedViewService = seedViewService;
        }

        [HttpPost("/test/create")]
        public IActionResult TestCreate()
        {
            Staff newAdmin = new Staff()
            {
                IsAdmin = true,
                FirstName = "Jean-Luc",
                LastName = "Picard",
                Role = "Admin",
                StaffUsername = "JPicardNumber1",
                Password = "password0$",
                MessagingLink = new MessagingLink()
            };
            PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
            newAdmin.Password = hasher.HashPassword(newAdmin, newAdmin.Password);

            _context.Staff.Add(newAdmin);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newAdmin.StaffId);
            HttpContext.Session.SetString("Name", newAdmin.FullName());
            HttpContext.Session.SetString("Role", newAdmin.Role);
            HttpContext.Session.SetInt32("MessageLinkId", newAdmin.MessagingLink.MessagingLinkId);
            
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/test/options")]
        public IActionResult TestLoginOptions(int staffId)
        {
            Staff staffmember = _context.Staff
                .Include(staff => staff.MessagingLink)
                .FirstOrDefault(s => s.StaffId == staffId);
                
            HttpContext.Session.SetInt32("UserId", staffmember.StaffId);
            HttpContext.Session.SetString("Name", staffmember.FullName());
            HttpContext.Session.SetString("Role", staffmember.Role);
            HttpContext.Session.SetInt32("MessageLinkId", staffmember.MessagingLink.MessagingLinkId);

            return RedirectToAction("Index", "Login");
        }
        
        [HttpGet("/test/seed")]
        public IActionResult Seed()
        {
            ViewBag.currentStaff = _context.Staff.Count();
            ViewBag.currentPatients = _context.Patients.Count();
            
            return View("SeedPrompt");
        }
        
        [HttpPost("/test/seed")]
        public IActionResult Seed(string option, SeedTestView seedAmount)
        {
            if(option == "seed")
            {                
                _seedViewService.SeedNStaff(seedAmount.Staff);

                _seedViewService.SeedNPatients(seedAmount.Patients);
            }

            return RedirectToAction("Index", "Login");
        }

    }
}