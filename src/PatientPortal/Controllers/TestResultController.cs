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
            TestResultFormView viewModel = _testResultViewService.ReturnTestResultFormView(patientId);
                
            return View("TestResultForm", viewModel);
        }

        [HttpPost("")]
        public IActionResult TestResultCreate(int patientId, TestResultFormView formData)
        {
            if(ModelState.IsValid)
            {
                _testResultService.CreateTestResult(patientId, (int)staffId, formData);
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }

            formData.Patient = _patientViewService.GetPatientInfoHeader(patientId);
            
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