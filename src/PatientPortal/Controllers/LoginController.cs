using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.DTOs;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/")]
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public IActionResult Index()
        {
            // If already authenticated, redirect based on role
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsAdmin())
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
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> StaffLogin(LoginStaff loginInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loginDTO = loginInfo.ToLoginStaffDTO();
                    var result = await _authService.SignInAsync(loginDTO.StaffUsername, loginDTO.LoginPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    result.AddErrorToModelState(ModelState);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the account.");
                }                
            }
            return View("StaffLogin");
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}