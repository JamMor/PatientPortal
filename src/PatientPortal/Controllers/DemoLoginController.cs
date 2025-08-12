using System;
using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PatientPortal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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
            if (result.Succeeded)
            {
                Staff newAdmin = result.Value;
                await _demoLoginService.LoginStaffById(newAdmin.StaffId);
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
            await _demoLoginService.LoginStaffById(staffId);

            return RedirectToAction("Index", "Login");
        }
    }
}