using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [AllowAnonymous]
    public class DemoLoginController : Controller
    {
        private IDemoLoginService _demoLoginService;

        public DemoLoginController(IDemoLoginService demoLoginService)
        {
            _demoLoginService = demoLoginService;
        }

        [HttpGet("demo/staff")]
        public IActionResult GetDemoStaffLoginOptions()
        {
            List<DemoLoginViewModel> allStaff = _demoLoginService.GetAllStaff();

            return PartialView("Views/Login/_DemoLogins.cshtml", allStaff);
        }

        [HttpPost("/demo/create")]
        public async Task<IActionResult> CreateDemoAdmin()
        {
            var result = await _demoLoginService.CreateAdmin();
            if (result.Succeeded && result.Value != null)
            {
                Staff newAdmin = result.Value;
                var loginResult = await _demoLoginService.LoginStaffById(newAdmin.StaffId);
                if (!loginResult)
                {
                    Console.WriteLine("Admin user created but failed to log in.");
                }
            }
            else
            {
                Console.WriteLine("Failed to create admin user.");
                Console.WriteLine(string.Join(", ", result.IdentityResult.Errors.Select(e => e.Description)));
                // result.AddErrorDictionaryToModelState(ModelState);
            }
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/demo/staff")]
        public async Task<IActionResult> LoginDemoStaff(int staffId)
        {
            var loginResult = await _demoLoginService.LoginStaffById(staffId);
            if (!loginResult)
            {
                Console.WriteLine($"Failed to log in staff with ID {staffId}.");
            }

            return RedirectToAction("Index", "Login");
        }
    }
}