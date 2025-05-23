using System;
using System.Linq;
using PatientPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Interfaces;

namespace PatientPortal.Controllers
{
    
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
        public IActionResult TestCreate()
        {
            LoginStaffDTO newAdmin = _testLoginService.CreateAdmin();

            HttpContext.Session.SetInt32("UserId", newAdmin.StaffId);
            HttpContext.Session.SetString("Name", newAdmin.FullName);
            HttpContext.Session.SetString("Role", newAdmin.Role);
            HttpContext.Session.SetInt32("MessageLinkId", newAdmin.MessagingLinkId);
            
            return RedirectToAction("StaffManager", "Staff");
        }

        [HttpPost("/test/options")]
        public IActionResult TestLoginOptions(int staffId)
        {
            LoginStaffDTO staffmember = _testLoginService.LoginStaffById(staffId);
                
            HttpContext.Session.SetInt32("UserId", staffmember.StaffId);
            HttpContext.Session.SetString("Name", staffmember.FullName);
            HttpContext.Session.SetString("Role", staffmember.Role);
            HttpContext.Session.SetInt32("MessageLinkId", staffmember.MessagingLinkId);

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