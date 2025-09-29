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
    [Route("/provider/patients/{patientId}/test")]
    public class TestResultController : Controller
    {
        private int? staffId => User.GetStaffId();

        private ITestResultService _testResultService;
        private ITestResultViewService _testResultViewService;
        public TestResultController(ITestResultService testResultService, ITestResultViewService testResultViewService)
        {
            _testResultService = testResultService;
            _testResultViewService = testResultViewService;
        }

//======================Create TestResult===========================
        [HttpGet("")]
        public IActionResult TestResultAdd(int patientId)
        {
            TestResultForm form = _testResultViewService.GetNewTestResultForm(patientId);
                
            return View("TestResultForm", form);
        }

        [HttpPost("")]
        public IActionResult TestResultCreate(int patientId, TestResultForm formData)
        {
            if(!staffId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve staff information. Please ensure you are logged in and try again.");
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var testResultDTO = formData.ToTestResultDTO();
                    _testResultService.CreateTestResult(patientId, (int)staffId, testResultDTO);
                    
                    return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the test result.");
                }
            }

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