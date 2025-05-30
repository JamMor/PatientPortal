using System;
using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PatientPortal.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        private ITestLoginService _testLoginService;
        private ISeedViewService _seedViewService;

        public TestController(ITestLoginService testLoginService, ISeedViewService seedViewService)
        {
            _testLoginService = testLoginService;
            _seedViewService = seedViewService;
        }

        [HttpGet("test/staff")]
        public IActionResult GetStaffLoginOptions()
        {
            List<TestLoginViewModel> allStaff = _testLoginService.GetAllStaff();

            if(allStaff.Count >= 0)
            {
                return Ok(new {Status = allStaff.Count, StaffInfo = allStaff, Message = $"Returned {allStaff.Count} staff"});
            }

            return NoContent();
        }

        [HttpPost("/test/create")]
        public async Task<IActionResult> TestCreate()
        {
            Staff newAdmin = await _testLoginService.CreateAdmin();
            if (newAdmin == null)
            {
                await _testLoginService.LoginStaffById(newAdmin.StaffId);
            }
            
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/test/options")]
        public async Task<IActionResult> TestLoginOptions(int staffId)
        {
            await _testLoginService.LoginStaffById(staffId);

            return RedirectToAction("Index", "Login");
        }
        
        [HttpGet("/test/seed")]
        public IActionResult Seed()
        {
            SeedFormView viewModel = _seedViewService.ReturnSeedFormView();
            
            return View("SeedPrompt", viewModel);
        }
        
        [HttpPost("/test/seed")]
        public IActionResult Seed(SeedFormView seedAmount)
        {             
            _seedViewService.SeedNStaff(seedAmount.Staff);
            _seedViewService.SeedNPatients(seedAmount.Patients);

            return RedirectToAction("Index", "Login");
        }

    }
}