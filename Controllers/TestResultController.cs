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
            ViewBag.patientId = patientId;
            ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
            return View("TestResultForm");
        }

        [HttpPost("")]
        public IActionResult TestResultCreate(int patientId, TestResult newTest, List<int> issues)
        {
            if(ModelState.IsValid)
            {
                newTest.PatientId = patientId;
                newTest.StaffId = (int)uuid;
                _context.TestResults.Add(newTest);
                _context.SaveChanges();

                foreach (int issueId in issues)
                {
                    TestHealthIssueAssociation newAssociation = new TestHealthIssueAssociation()
                    {
                        TestResultId = newTest.TestResultId,
                        HealthIssueId = issueId
                    };
                    _context.TestHealthIssueAssociations.Add(newAssociation);
                    _context.SaveChanges();
                }
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            ViewBag.patientId = patientId;
            ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
            return View("TestResultForm");
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