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
    [Route("/provider/patients/{patientId}/test")]
    public class TestResultController : Controller
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
        private ITestResultService _testResultService;
        public TestResultController(PatientPortalContext context, ITestResultService testResultService)
        {
            _context = context;
            _testResultService = testResultService;
        }

//======================Create TestResult===========================
        [HttpGet("")]
        public IActionResult TestResultAdd(int patientId)
        {
            
            TestResultFormView viewModel = _context.Patients
                .Where(p => p.PatientId == patientId)
                .Include(p => p.HealthIssues)
                .Select(p => new TestResultFormView()
                {
                    Patient = new PatientHeaderInfoView()
                    {
                        CurrentPatientId = p.PatientId,
                        CurrentPatientLinkId = p.MessagingLink.MessagingLinkId,
                        CurrentPatientFirstName = p.FirstName,
                        CurrentPatientLastName = p.LastName,
                        CurrentPatientSSN = p.Last4SSN,
                        CurrentPatientDOB = p.DOB,
                        CurrentPatientAge = p.Age,
                        CurrentPatientCreatedOn = p.CreatedAt
                    },
                    HealthIssues = p.HealthIssues
                        .Select(h => new HealthIssueCheckbox()
                        {
                            HealthIssueId = h.HealthIssueId,
                            ShortDescription = h.ShortDescription,
                            CreatedAt = h.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefault();
                
            return View("TestResultForm", viewModel);
        }

        [HttpPost("")]
        public IActionResult TestResultCreate(int patientId, TestResultFormView formData, List<int> issues)
        {
            if(ModelState.IsValid)
            {
                _testResultService.CreateTestResult(patientId, (int)uuid, formData);
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }

            formData.Patient = _context.Patients
                    .Where(p => p.PatientId == patientId)
                    .Select(p => new PatientHeaderInfoView()
                    {
                        CurrentPatientId = p.PatientId,
                        CurrentPatientFirstName = p.FirstName,
                        CurrentPatientLastName = p.LastName,
                        CurrentPatientSSN = p.Last4SSN,
                        CurrentPatientDOB = p.DOB,
                        CurrentPatientAge = p.Age,
                        CurrentPatientCreatedOn = p.CreatedAt
                    })
                    .FirstOrDefault();
            
            return View("TestResultForm", formData);
        }
    
        [HttpPost("{testId}/delete")]
        public IActionResult TestResultDelete(int patientId, int testId)
        {
            _testResultService.DeleteTestResult(testId);
            
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}