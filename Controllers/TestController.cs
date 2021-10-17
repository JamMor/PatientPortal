using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult TestCreate(string role)
        {
            Staff newAdmin = new Staff()
            {
                IsAdmin = true,
                FirstName = "Jean-Luc",
                LastName = "Picard",
                Role = "Admin",
                StaffUsername = "JPicardNumber1",
                Password = "password0$"
            };
            PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
            newAdmin.Password = hasher.HashPassword(newAdmin, newAdmin.Password);

            _context.Staff.Add(newAdmin);
            _context.SaveChanges();

            MessagingLink newLink = new MessagingLink()
            {
                StaffId = newAdmin.StaffId
            };

            _context.MessagingLinks.Add(newLink);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", newAdmin.StaffId);
            HttpContext.Session.SetString("Name", newAdmin.FullName());
            HttpContext.Session.SetString("Role", newAdmin.Role);
            
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/test/options")]
        public IActionResult TestLoginOptions(int staffId)
        {
            Staff staffmember = _context.Staff.FirstOrDefault(s => s.StaffId == staffId);
            HttpContext.Session.SetInt32("UserId", staffmember.StaffId);
            HttpContext.Session.SetString("Name", staffmember.FullName());
            HttpContext.Session.SetString("Role", staffmember.Role);

            return RedirectToAction("Index", "Login");
        }

    }
}