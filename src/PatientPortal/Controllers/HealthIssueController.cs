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
        private IHealthIssueService _healthIssueService;
        public HealthIssueController(IHealthIssueService healthIssueService)
        {
            _healthIssueService = healthIssueService;
        }

        //=====================Create HealthIssue===========================
        [HttpGet("")]
        public IActionResult HealthIssueAdd()
        {
            return View("HealthIssueForm", new HealthIssueForm());
        }
        
        [HttpPost("")]
        public IActionResult HealthIssueCreate(int patientId, HealthIssueForm formData)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var healthIssueDTO = formData.ToHealthIssueDTO();
                    _healthIssueService.CreateHealthIssue(patientId, healthIssueDTO);
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the health issue.");
                }
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }

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