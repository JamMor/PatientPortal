using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//Only needed for displaying staff logins for testing
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PatientPortal.Controllers
{
    [Route("/")]
    public class LoginController : Controller
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
        public LoginController(PatientPortalContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if(IsLoggedIn)
            {
                if(HttpContext.Session.GetString("Role") == "Admin")
                {
                    return RedirectToAction("StaffManager", "Staff");
                }
                else
                {
                    return RedirectToAction("PatientManager", "Patients");
                }
                
            }
            //========================================================
            //=================For Test Logins========================
            List<Staff> allStaff = _context.Staff.ToList();
            //========================================================
            return View("StaffLogin", allStaff);
        }
        
        [HttpPost("login")]
        public IActionResult StaffLogin(LoginStaff loginInfo)
        {
            if(ModelState.IsValid)
            {
                Staff savedStaff = _context.Staff
                    .Include(staff => staff.MessagingLink)
                    .FirstOrDefault(staff => staff.StaffUsername == loginInfo.StaffUsername);
                if(savedStaff != null)
                {
                    PasswordHasher<LoginStaff> hasher = new PasswordHasher<LoginStaff>();
                    PasswordVerificationResult passwordVerification = hasher.VerifyHashedPassword(loginInfo, savedStaff.Password, loginInfo.LoginPassword);
                    if(passwordVerification != 0)
                    {
                        HttpContext.Session.SetInt32("UserId", savedStaff.StaffId);
                        HttpContext.Session.SetString("Name", savedStaff.FullName());
                        HttpContext.Session.SetString("Role", savedStaff.Role);
                        HttpContext.Session.SetInt32("MessageLinkId", savedStaff.MessagingLink.MessagingLinkId);

                        if(savedStaff.IsAdmin)
                        {
                            return RedirectToAction("StaffManager", "Staff");
                        }
                        else
                        {
                            return RedirectToAction("PatientManager", "Patients");
                        }
                    }
                }
                ModelState.AddModelError("LoginPassword", "Incorrect login info.");
            }
            return View("StaffLogin");
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}