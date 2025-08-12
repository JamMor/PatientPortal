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
    public class TestController : Controller
    {
        private ITestLoginService _testLoginService;

        public TestController(ITestLoginService testLoginService)
        {
            _testLoginService = testLoginService;
        }

        [HttpGet("test/staff")]
        public IActionResult GetStaffLoginOptions()
        {
            List<TestLoginViewModel> allStaff = _testLoginService.GetAllStaff();

            return PartialView("Views/Login/_TestingLogins.cshtml", allStaff);
        }

        [HttpPost("/test/create")]
        public async Task<IActionResult> TestCreate()
        {
            var result = await _testLoginService.CreateAdmin();
            if (result.Succeeded)
            {
                Staff newAdmin = result.Value;
                await _testLoginService.LoginStaffById(newAdmin.StaffId);
            }
            else
            {
                Console.WriteLine("Failed to create admin user.");
                Console.WriteLine(string.Join(", ", result.IdentityResult.Errors.Select(e => e.Description)));
                // result.AddErrorDictionaryToModelState(ModelState);
            }
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/test/options")]
        public async Task<IActionResult> TestLoginOptions(int staffId)
        {
            await _testLoginService.LoginStaffById(staffId);

            return RedirectToAction("Index", "Login");
        }
    }
}