using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

namespace PatientPortal.Controllers
{
    [Route("/provider/staff")]
    public class StaffController : Controller
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

        private IStaffService _staffService;
        private IStaffViewService _staffViewService;
        private IStaffRegistrationService _staffRegistrationService;

        public StaffController(
            IStaffService staffService,
            IStaffViewService staffViewService,
            IStaffRegistrationService staffRegistrationService)
        {
            _staffService = staffService;
            _staffViewService = staffViewService;
            _staffRegistrationService = staffRegistrationService;
        }

//==============Staff Manager==============================
        [HttpGet("")]
        public IActionResult StaffManager(StaffSearch searchBar, Paginator paginationSettings)
        {
            StaffManagerView viewModel = _staffViewService.ReturnStaffManagerView(searchBar, paginationSettings);

            return View("StaffManager", viewModel);
        }

        [HttpGet("add")]
        public IActionResult StaffAdd()
        {
            return View("StaffForm");
        }

        [HttpPost("add")]
        public async Task<IActionResult> StaffCreate(StaffFormView staffFormView)
        {
            if (ModelState.IsValid)
            {
                var result = await _staffRegistrationService.RegisterStaffAsync(staffFormView);

                if (result.Succeeded)
                {
                    return RedirectToAction("StaffInfo", "Staff", new { staffId = result.Value.StaffId });
                }

                result.AddErrorDictionaryToModelState(ModelState,
                    usernameField: nameof(StaffFormView.StaffUsername),
                    passwordField: nameof(StaffFormView.Password),
                    confirmPasswordField: nameof(StaffFormView.ConfirmPassword)
                );
            }
            return View("StaffForm");
        }

        [HttpGet("{staffId}")]
        public IActionResult StaffInfo(int staffId)
        {
            StaffInfoViewModel staffInfo = _staffViewService.GetStaffInfo(staffId);

            return View("StaffInfo", staffInfo);
        }
    

        [HttpPost("{staffId}/delete")]
        public IActionResult StaffDelete(int staffId)
        {
            _staffService.DeleteStaff(staffId);
            
            return RedirectToAction("StaffManager", "Staff");
        }

    }
}