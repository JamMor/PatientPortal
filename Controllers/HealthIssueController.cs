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

        private PatientPortalContext _context;
        private IHealthIssueService _healthIssueService;
        public HealthIssueController(PatientPortalContext context, IHealthIssueService healthIssueService)
        {
            _context = context;
            _healthIssueService = healthIssueService;
        }

        //=====================Create HealthIssue===========================
        [HttpGet("")]
        public IActionResult HealthIssueAdd(int patientId)
        {
            ViewBag.patientId = patientId;
            return View("HealthIssueForm");
        }
        
        [HttpPost("")]
        public IActionResult HealthIssueCreate(int patientId, HealthIssue newIssue)
        {
            if(ModelState.IsValid)
            {
                _healthIssueService.CreateHealthIssue(patientId, newIssue);

                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            ViewBag.patientId = patientId;
            return View("HealthIssueForm");
        }
        

        [HttpPost("{issueId}/delete")]
        public IActionResult IssueDelete(int patientId, int issueId)
        {
            _healthIssueService.DeleteHealthIssue(patientId, issueId);

            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}