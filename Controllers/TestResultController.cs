using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public TestResultController(PatientPortalContext context)
        {
            _context = context;
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
                TestResult testData = new TestResult()
                {
                    Type = formData.TestResult.Type,
                    Comment = formData.TestResult.Comment,
                    PatientId = patientId,
                    StaffId = (int)uuid,
                    AssociatedHealthIssues = formData.HealthIssues
                    .Where(h => h.Selected == true)
                    .Select(h => new TestHealthIssueAssociation()
                    {
                        HealthIssueId = h.HealthIssueId
                    })
                    .ToList()
                };

                _context.TestResults.Add(testData);
                _context.SaveChanges();
                // foreach (int issueId in issues)
                // {
                //     TestHealthIssueAssociation newAssociation = new TestHealthIssueAssociation()
                //     {
                //         TestResultId = testData.TestResultId,
                //         HealthIssueId = issueId
                //     };
                //     _context.TestHealthIssueAssociations.Add(newAssociation);
                //     _context.SaveChanges();
                // }
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
            TestResult deletedTestResult = _context.TestResults.SingleOrDefault(issue => issue.TestResultId == testId);
            if(deletedTestResult != null)
            {
                _context.TestResults.Remove(deletedTestResult);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}