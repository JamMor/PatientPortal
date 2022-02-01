using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;

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
        private ILoginService _loginService;
        public LoginController(PatientPortalContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
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
                LoginStaffDTO staffUser = _loginService.AttemptStaffLogin(loginInfo);
                if(staffUser != null)
                {
                    HttpContext.Session.SetInt32("UserId", staffUser.StaffId);
                    HttpContext.Session.SetString("Name", staffUser.FullName);
                    HttpContext.Session.SetString("Role", staffUser.Role);
                    HttpContext.Session.SetInt32("MessageLinkId", staffUser.MessagingLinkId);

                    return RedirectToAction("Index");
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