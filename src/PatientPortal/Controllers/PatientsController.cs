using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Authorization;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;

namespace PatientPortal.Controllers
{
    [Authorize(Policy = PolicyNames.ManagePatients)]
    [Route("/provider/patients")]
    public class PatientsController : Controller
    {
        private int? staffId => User.GetStaffId();

        private IPatientService _patientService;
        private IPatientViewService _patientViewService;
        private IPatientStaffConnectionService _patientStaffConnectionService;
        public PatientsController(IPatientService patientService, IPatientViewService patientViewService, IPatientStaffConnectionService patientStaffConnectionService)
        {
            _patientService = patientService;
            _patientViewService = patientViewService;
            _patientStaffConnectionService = patientStaffConnectionService;
        }

        //===========================Patient Manager==============================
        [HttpGet("")]
        public IActionResult PatientManager(PatientSearch searchBar, Paginator paginationSettings)
        {
            PatientManagerView viewModel = _patientViewService.ReturnPatientManagerView(searchBar, paginationSettings, (int)staffId);

            return View("PatientManager", viewModel);
        }

        [HttpGet("add")]
        public IActionResult PatientAdd()
        {
            return View("PatientForm");
        }

        [HttpPost("add")]
        public IActionResult PatientCreate(PatientFormView patientFormView)
        {
            if (ModelState.IsValid)
            {
                if (!_patientService.DoesPatientExist(patientFormView))
                {

                    int patientId = _patientService.CreatePatient(patientFormView);

                    return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
                }

                else
                {
                    ViewBag.AlreadyExistsError = "A patient already exists with this information.";
                }
            }
            return View("PatientForm");
        }

        [HttpGet("{patientId}")]
        public IActionResult PatientInfo(int patientId)
        {
            PatientInfoViewModel patientInfo = _patientViewService.GetPatientInfo(patientId);

            return View("PatientInfo", patientInfo);
        }

        [HttpPost("{patientId}/delete")]
        public IActionResult PatientDelete(int patientId)
        {
            _patientService.DeletePatient(patientId);

            return RedirectToAction("PatientManager", "Patients");
        }

        //======================Medical Team================================
        [HttpPost("{patientId}/join")]
        public IActionResult MedicalTeamJoin(int patientId)
        {
            _patientStaffConnectionService.AddStaffToPatientTeam(patientId, (int)staffId);
            
            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
        [HttpPost("{patientId}/leave")]
        public IActionResult MedicalTeamLeave(int patientId)
        {
            _patientStaffConnectionService.RemoveStaffFromPatientTeam(patientId, (int)staffId);

            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
    }
}