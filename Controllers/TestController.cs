using System;
using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Bogus;

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
        public TestController(PatientPortalContext context)
        {
            _context = context;
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
                var staffFaker = new Faker<Staff>()
                    .StrictMode(false)
                    .Rules((f, s) => 
                    {
                        s.IsAdmin = false;
                        s.FirstName = f.Name.FirstName();
                        s.LastName = f.Name.LastName();
                        s.Role = f.PickRandomParam("MD", "RN");
                        s.StaffUsername = f.Internet.UserName(s.FirstName, s.LastName)+"00$";
                        if(s.StaffUsername.Length < 10)
                        {
                            s.StaffUsername += f.Random.String2(10-s.StaffUsername.Length, "0123456789");
                        };
                        s.Password = "Password0$";
                        PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
                        s.Password = hasher.HashPassword(s, s.Password);
                    });
                
                var addressFaker = new Faker<Address>()
                    .StrictMode(false)
                    .Rules((f, a) => 
                    {
                        a.StreetAddress = f.Address.StreetAddress();
                        a.City = f.Address.City();
                        a.State = f.Address.State();
                        a.ZipCode = f.Address.ZipCode();
                    });

                var patientFaker = new Faker<Patient>()
                    .StrictMode(false)
                    .Rules((f, p) => 
                    {
                        p.FirstName = f.Name.FirstName();
                        p.LastName = f.Name.LastName();
                        p.DOB = f.Date.Between(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-18));
                        p.Last4SSN = f.Random.String2(4, "0123456789");
                        p.Email = f.Internet.Email(p.FirstName, p.LastName);
                        p.CreatedAt = f.Date.Between(DateTime.Today.AddYears(-30), DateTime.Today);
                        if(0 == f.Random.Number(3))
                        {
                            p.Address = addressFaker.Generate();
                        }
                    });
                

                
                var fakeStaff = staffFaker.Generate(seedAmount.Staff);
                var fakePatients = patientFaker.Generate(seedAmount.Patients);

                _context.Staff.AddRange(fakeStaff);
                _context.Patients.AddRange(fakePatients);

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Login");
        }

    }
}