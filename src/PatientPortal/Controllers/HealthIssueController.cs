using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Authorization;
using PatientPortal.DTOs;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Authorize(Policy = PolicyNames.ManagePatients)]
    [Route("/provider/patients/{patientId}/issue")]
    public class HealthIssueController : Controller
    {
        private IPatientViewService _patientViewService;
        private IHealthIssueService _healthIssueService;
        public HealthIssueController(IPatientViewService patientViewService, IHealthIssueService healthIssueService)
        {
            _patientViewService = patientViewService;
            _healthIssueService = healthIssueService;
        }

        //=====================Create HealthIssue===========================
        [HttpGet("")]
        public IActionResult HealthIssueAdd(int patientId)
        {
            var patientHeader = _patientViewService.GetPatientInfoHeader(patientId);
            if (patientHeader == null)
            {
                return NotFound();
            }

            HealthIssueFormView viewModel = new HealthIssueFormView()
                {
                    Patient = patientHeader,
                    HealthIssueForm = new HealthIssueForm()
                };

            return View("HealthIssueForm", viewModel);
        }
        
        [HttpPost("")]
        public IActionResult HealthIssueCreate(int patientId, HealthIssueFormView formData)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var healthIssueDTO = formData.HealthIssueForm.ToHealthIssueDTO();
                    _healthIssueService.CreateHealthIssue(patientId, healthIssueDTO);
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the health issue.");
                }
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            
            var patientHeader = _patientViewService.GetPatientInfoHeader(patientId);
            if (patientHeader == null)
            {
                return NotFound();
            }
            formData.Patient = patientHeader;

            return View("HealthIssueForm", formData);
        }
        

        [HttpPost("{issueId}/delete")]
        public IActionResult IssueDelete(int patientId, int issueId)
        {
            _healthIssueService.DeleteHealthIssue(patientId, issueId);

            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}