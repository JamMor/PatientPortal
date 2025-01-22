using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

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
        public StaffController(IStaffService staffService, IStaffViewService staffViewService)
        {
            _staffService = staffService;
            _staffViewService = staffViewService;
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
        public IActionResult StaffCreate(StaffFormView staffFormView)
        {
            if(!_staffService.DoesStaffExist(staffFormView.StaffUsername))
            {
                if(ModelState.IsValid)
                {
                    int staffId = _staffService.CreateStaff(staffFormView);

                    return RedirectToAction("StaffInfo", "Staff", new { staffId = staffId });
                }
            }
            else
            {
                ModelState.AddModelError("StaffUsername","This username is already taken.");
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