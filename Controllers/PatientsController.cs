using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public PatientsController(PatientPortalContext context)
        {
            _context = context;
        }

        //===========================Patient Manager==============================
        [HttpGet("")]
        public IActionResult PatientManager()
        {
            PatientSearch EmptySearch = new PatientSearch();

            List<Patient> AllResults = _context.Patients
                .ToList();

            PatientManagerView ViewModel = new PatientManagerView
            {
                SearchBar = EmptySearch,
                SearchResults = AllResults
            };

            return View("PatientManager", ViewModel);
        }

        [HttpPost("")]
        public IActionResult PatientManagerQuery(PatientSearch PatientQuery)
        {
            List<Patient> QueryResults = _context.Patients
                .Where(patient => PatientQuery.SearchPatientId == null || patient.PatientId == PatientQuery.SearchPatientId)
                .Where(patient => string.IsNullOrEmpty(PatientQuery.SearchFirstName) || patient.FirstName.StartsWith(PatientQuery.SearchFirstName))
                .Where(patient => string.IsNullOrEmpty(PatientQuery.SearchLastName) || patient.LastName.StartsWith(PatientQuery.SearchLastName))
                .Where(patient => string.IsNullOrEmpty(PatientQuery.SearchSSN) || patient.Last4SSN == PatientQuery.SearchSSN)
                .Where(patient => PatientQuery.SearchBirthdate == null || patient.DOB == PatientQuery.SearchBirthdate)
                .ToList();

            PatientManagerView ViewModel = new PatientManagerView
            {
                SearchBar = PatientQuery,
                SearchResults = QueryResults
            };

            return View("PatientManager", ViewModel);
        }

        [HttpGet("add")]
        public IActionResult PatientAdd()
        {
            return View("PatientForm");
        }

        [HttpPost("add")]
        public IActionResult PatientCreate(NewPatientInput newPatientInput)
        {

            bool IsNewPatientNull = newPatientInput.Patient is null;
            bool IsNewAddressNull = newPatientInput.Address is null;

            if (newPatientInput.Address.StreetAddress == null
                && newPatientInput.Address.City == null
                && newPatientInput.Address.State == null
                && newPatientInput.Address.ZipCode == null)
            {
                newPatientInput.Address = null;
            }
            else if(!(newPatientInput.Address.StreetAddress != null
                && newPatientInput.Address.City != null
                && newPatientInput.Address.State != null
                && newPatientInput.Address.ZipCode != null))
            {
                TempData["AddressError"] = "Complete all fields if entering address.";
            }

            if (ModelState.IsValid && TempData["AddressError"] == null)
            {
                Patient newPatient = newPatientInput.Patient;
                Address newAddress = newPatientInput.Address;

                if (!_context.Patients.Any(patient =>
                    patient.Last4SSN == newPatient.Last4SSN
                    && patient.DOB == newPatient.DOB
                    && patient.FirstName == newPatient.FirstName
                    && patient.LastName == newPatient.LastName))
                {

                    MessagingLink newLink = new MessagingLink()
                    {
                    };
                    // _context.MessagingLinks.Add(newLink);
                    newPatient.MessagingLink = newLink;
                    
                    _context.Patients.Add(newPatient);
                    
                    if(newAddress != null)
                    {
                        newPatient.Address = newAddress;
                    }
                    
                    _context.SaveChanges();

                    return RedirectToAction("PatientManager", "Patients");
                }

                else
                {
                    ModelState.AddModelError("Last4SSN", "A patient already exists with these criteria.");
                }
            }
            return View("PatientForm");
        }

        [HttpGet("{patientId}")]
        public IActionResult PatientInfo(int patientId)
        {
            Patient patient = _context.Patients
                .Include(patient => patient.HealthIssues)
                .ThenInclude(issue => issue.AssociatedTestResults)
                .Include(patient => patient.HealthIssues)
                .ThenInclude(issue => issue.AssociatedVisits)
                .Include(patient => patient.Visits)
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.Tests)
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.MedicalTeam)
                .ThenInclude(team => team.Staff)
                .FirstOrDefault(patient => patient.PatientId == patientId);
            return View("PatientInfo", patient);
        }

        [HttpPost("{patientId}/delete")]
        public IActionResult PatientDelete(int patientId)
        {
            Patient deletedPatient = _context.Patients.SingleOrDefault(patient => patient.PatientId == patientId);
            if (deletedPatient != null)
            {
                _context.Patients.Remove(deletedPatient);
                _context.SaveChanges();
            }
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