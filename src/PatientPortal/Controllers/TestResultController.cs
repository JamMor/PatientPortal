using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Extensions;
using Microsoft.AspNetCore.Authorization;
using PatientPortal.Authorization;
using PatientPortal.DTOs;

namespace PatientPortal.Controllers
{
    [Authorize(Policy = PolicyNames.ManagePatients)]
    [Route("/provider/patients/{patientId}/test")]
    public class TestResultController : Controller
    {
        private int? staffId => User.GetStaffId();

        private IPatientViewService _patientViewService;
        private ITestResultService _testResultService;
        private ITestResultViewService _testResultViewService;
        public TestResultController(IPatientViewService patientViewService, ITestResultService testResultService, ITestResultViewService testResultViewService)
        {
            _patientViewService = patientViewService;
            _testResultService = testResultService;
            _testResultViewService = testResultViewService;
        }

//======================Create TestResult===========================
        [HttpGet("")]
        public IActionResult TestResultAdd(int patientId)
        {
            TestResultFormView? viewModel = _testResultViewService.ReturnTestResultFormView(patientId);
            if(viewModel == null)
            {
                return NotFound();
            }
                
            return View("TestResultForm", viewModel);
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

            var patientHeader = _patientViewService.GetPatientInfoHeader(patientId);
            if (patientHeader == null)
            {
                return NotFound();
            }

            TestResultFormView viewModel = new TestResultFormView()
            {
                Patient = patientHeader,
                TestResultForm = formData
            };
            
            return View("TestResultForm", viewModel);
        }
    
        [HttpPost("{testId}/delete")]
        public IActionResult TestResultDelete(int patientId, int testId)
        {
            _testResultService.DeleteTestResult(testId);
            
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}