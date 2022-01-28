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
    [Route("/provider/patients")]
    public class PatientsController : Controller
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
        private IPatientService _patientService;
        public PatientsController(PatientPortalContext context, IPatientService patientService)
        {
            _context = context;
            _patientService = patientService;
        }

        //===========================Patient Manager==============================
        [HttpGet("")]
        public IActionResult PatientManager(PatientSearch SearchBar, ListResultAttributes DisplayProperties)
        {

            PatientManagerView ViewModel = _patientService.GetPatientbyQuery(SearchBar, DisplayProperties);

            return View("PatientManager", ViewModel);
        }

        [HttpGet("add")]
        public IActionResult PatientAdd()
        {
            return View("PatientForm");
        }

        [HttpPost("add")]
        public IActionResult PatientCreate(PatientFormView patientFormView)
        {
            if (ModelState.IsValid)
            {
                if (!_patientService.DoesPatientExist(patientFormView))
                {

                    int patientId = _patientService.CreatePatient(patientFormView);

                    return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
                }

                else
                {
                    ViewBag.AlreadyExistsError = "A patient already exists with this information.";
                }
            }
            return View("PatientForm");
        }

        [HttpGet("{patientId}")]
        public IActionResult PatientInfo(int patientId)
        {
            Patient patient = _patientService.GetPatientbyId(patientId);

            return View("PatientInfo", patient);
        }

        [HttpPost("{patientId}/delete")]
        public IActionResult PatientDelete(int patientId)
        {
            _patientService.DeletePatient(patientId);

            return RedirectToAction("PatientManager", "Patients");
        }

        //======================Medical Team================================
        [HttpPost("{patientId}/join")]
        public IActionResult MedicalTeamJoin(int patientId)
        {
            PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);

            if (oldLink == null)
            {
                PatientStaffConnection newLink = new PatientStaffConnection()
                {
                    PatientId = patientId,
                    StaffId = (int)uuid
                };
                _context.PatientStaffConnections.Add(newLink);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
        [HttpPost("{patientId}/leave")]
        public IActionResult MedicalTeamLeave(int patientId)
        {
            PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);

            if (oldLink != null)
            {
                _context.PatientStaffConnections.Remove(oldLink);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new { patientId = patientId });
        }
    }
}