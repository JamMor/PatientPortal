using System.Threading.Tasks;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Extensions;

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

        private readonly IAuthService _authService;
        public LoginController(IAuthService authService)
        {
            _authService = authService;
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
            return View("StaffLogin");
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> StaffLogin(LoginStaff loginInfo)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.SignInAsync(loginInfo.StaffUsername, loginInfo.LoginPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                
                result.AddErrorToModelState(ModelState);
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