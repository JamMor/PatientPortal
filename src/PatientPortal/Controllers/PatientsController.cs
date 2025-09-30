using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Authorization;
using PatientPortal.DTOs;
using PatientPortal.Extensions;
using PatientPortal.Interfaces;
using PatientPortal.Models;

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
            if(!staffId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve staff information. Please ensure you are logged in and try again.");
            }

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
                try
                {
                    var patientDTO = patientFormView.ToPatientDTO();
                    
                    if (!_patientService.DoesPatientExist(patientDTO))
                    {

                        int patientId = _patientService.CreatePatient(patientDTO);

                        return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "A patient already exists with this information.");
                    }
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the visit.");
                }
            }
            return View("PatientForm", patientFormView);
        }

        [HttpGet("{patientId}")]
        public IActionResult PatientInfo(int patientId)
        {
            PatientInfoViewModel? patientInfo = _patientViewService.GetPatientInfo(patientId);
            if (patientInfo == null)
            {
                return NotFound();
            }

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
            if(!staffId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve staff information. Please ensure you are logged in and try again.");
            }
            
            _patientStaffConnectionService.AddStaffToPatientTeam(patientId, (int)staffId);
            
            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
        [HttpPost("{patientId}/leave")]
        public IActionResult MedicalTeamLeave(int patientId)
        {
            if(!staffId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve staff information. Please ensure you are logged in and try again.");
            }
            
            _patientStaffConnectionService.RemoveStaffFromPatientTeam(patientId, (int)staffId);

            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
    }
}