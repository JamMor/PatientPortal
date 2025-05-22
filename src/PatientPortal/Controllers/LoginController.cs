using System.Threading.Tasks;
using PatientPortal.Models;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Interfaces;
using PatientPortal.Extensions;

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
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}