#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Authorization;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;
using PatientPortal.DTOs;
using System;

namespace PatientPortal.Controllers
{
    [Authorize(Policy = PolicyNames.ManageStaff)]
    [Route("/provider/staff")]
    public class StaffController : Controller
    {
        private int? staffId => User.GetStaffId();

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
                try
                {
                    var staffAccountDTO = staffFormView.ToAccountDTO();
                    var staffDTO = staffFormView.ToStaffDTO();

                    var result = await _staffRegistrationService.RegisterStaffAsync(
                        staffAccountDTO,
                        staffDTO
                    );

                    if (result.Succeeded && result.Value != null)
                    {
                        return RedirectToAction(
                            "StaffInfo",
                            "Staff",
                            new { staffId = result.Value.StaffId }
                        );
                    }

                    result.AddErrorDictionaryToModelState(
                        ModelState,
                        usernameField: nameof(StaffFormView.StaffUsername),
                        passwordField: nameof(StaffFormView.Password),
                        confirmPasswordField: nameof(StaffFormView.ConfirmPassword)
                    );
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(
                        string.Empty,
                        "An unexpected error occurred while creating the staff member."
                    );
                }
            }

            return View("StaffForm");
        }

        [HttpGet("{staffId}")]
        public IActionResult StaffInfo(int staffId)
        {
            StaffInfoViewModel? staffInfo = _staffViewService.GetStaffInfo(staffId);
            if (staffInfo == null)
            {
                return NotFound();
            }

            return View("StaffInfo", staffInfo);
        }
    

        [HttpPost("{staffId}/delete")]
        public async Task<IActionResult> StaffDelete(int staffId)
        {
            await _staffRegistrationService.DeleteStaffAsync(staffId);
            
            return RedirectToAction("StaffManager", "Staff");
        }

    }
}