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
    [Route("/provider/patients/{patientId}/issue")]
    public class HealthIssueController : Controller
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
            HealthIssueFormView viewModel = new HealthIssueFormView()
                {
                    Patient = _patientViewService.GetPatientInfoHeader(patientId)
                };

            return View("HealthIssueForm", viewModel);
        }
        
        [HttpPost("")]
        public IActionResult HealthIssueCreate(int patientId, HealthIssueFormView formData)
        {
            if(ModelState.IsValid)
            {
                _healthIssueService.CreateHealthIssue(patientId, formData.HealthIssue);

                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            
            formData.Patient = _patientViewService.GetPatientInfoHeader(patientId);

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