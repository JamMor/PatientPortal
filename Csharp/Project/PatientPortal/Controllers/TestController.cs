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

        [HttpPost("/test")]
        public IActionResult TestLogin(string role)
        {
            if(role == "Admin")
            {
                HttpContext.Session.SetInt32("UserId", 1);
                HttpContext.Session.SetString("Name", "Jean-Luc Picard");
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("StaffManager", "Staff");
            }
            else if(role == "Data")
            {
                HttpContext.Session.SetInt32("UserId", 5);
                HttpContext.Session.SetString("Name", "Data Dat-erson");
                HttpContext.Session.SetString("Role", "MD");
                return RedirectToAction("PatientManager", "Staff");
            }
            else if(role == "Wes")
            {
                HttpContext.Session.SetInt32("UserId", 3);
                HttpContext.Session.SetString("Name", "Wesley Crusher");
                HttpContext.Session.SetString("Role", "NP");
                return RedirectToAction("PatientManager", "Staff");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}