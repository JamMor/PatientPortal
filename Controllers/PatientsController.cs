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
        public IActionResult PatientManager(PatientSearch SearchBar, ListResultAttributes DisplayProperties)
        {

            var patientQuery = _context.Patients
                .Where(patient => SearchBar.SearchPatientId == null || patient.PatientId == SearchBar.SearchPatientId)
                .Where(patient => string.IsNullOrEmpty(SearchBar.SearchFirstName) || patient.FirstName.StartsWith(SearchBar.SearchFirstName))
                .Where(patient => string.IsNullOrEmpty(SearchBar.SearchLastName) || patient.LastName.StartsWith(SearchBar.SearchLastName))
                .Where(patient => string.IsNullOrEmpty(SearchBar.SearchSSN) || patient.Last4SSN == SearchBar.SearchSSN)
                .Where(patient => SearchBar.SearchBirthdate == null || patient.DOB == SearchBar.SearchBirthdate);

            switch (DisplayProperties.SortOrder)
            {
                case "PatientId_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.PatientId);
                    break;
                case "PatientId_asc":
                    patientQuery = patientQuery.OrderBy(p => p.PatientId);
                    break;
                case "LastName_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.LastName);
                    break;
                case "LastName_asc":
                    patientQuery = patientQuery.OrderBy(p => p.LastName);
                    break;
                case "DOB_desc":
                    patientQuery = patientQuery.OrderByDescending(p => p.DOB);
                    break;
                case "DOB_asc":
                    patientQuery = patientQuery.OrderBy(p => p.DOB);
                    break;
                default:
                    patientQuery = patientQuery.OrderBy(s => s.LastName);
                    break;
            }

            DisplayProperties.ResultsCount = patientQuery.Count();
            
            List<Patient> queryResults = patientQuery
                .Skip(DisplayProperties.ResultsPerPage*(DisplayProperties.CurrentPage-1))
                .Take(DisplayProperties.ResultsPerPage)
                .ToList();

            PatientManagerView ViewModel = new PatientManagerView
            {
                SearchBar = SearchBar,
                SearchResults = queryResults,
                DisplayProperties = DisplayProperties
            };

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

                //Checks to ensure no patient with ALL same data already exists
                if (!_context.Patients.Any(patient =>
                    patient.Last4SSN == patientFormView.Last4SSN
                    && patient.DOB == patientFormView.DOB
                    && patient.FirstName == patientFormView.FirstName
                    && patient.LastName == patientFormView.LastName))
                {

                    Patient newPatient = new Patient()
                    {
                        FirstName = patientFormView.FirstName,
                        LastName = patientFormView.LastName,
                        DOB = patientFormView.DOB,
                        Last4SSN = patientFormView.Last4SSN,
                        PhoneNumber = patientFormView.PhoneNumber,
                        Email = patientFormView.Email,
                        MessagingLink = new MessagingLink()
                    };
                    
                    if(patientFormView.Address != null)
                    {
                        newPatient.Address = new Address()
                        {
                            StreetAddress = patientFormView.Address.StreetAddress,
                            City = patientFormView.Address.City,
                            State = patientFormView.Address.State,
                            ZipCode = patientFormView.Address.ZipCode
                        };
                    }

                    _context.Patients.Add(newPatient);
                    _context.SaveChanges();

                    return RedirectToAction("PatientInfo", "Patients", new { patientId = newPatient.PatientId });
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
            Patient patient = _context.Patients
                .Include(patient => patient.HealthIssues
                    .OrderByDescending(h => h.UpdatedAt))
                .ThenInclude(issue => issue.AssociatedTestResults)
                .Include(patient => patient.HealthIssues)
                .ThenInclude(issue => issue.AssociatedVisits)
                .Include(patient => patient.Visits
                    .OrderByDescending(v => v.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.Tests
                    .OrderByDescending(t => t.CreatedAt))
                .ThenInclude(team => team.Staff)
                .Include(patient => patient.MedicalTeam)
                .ThenInclude(team => team.Staff)
                .Include(p => p.MessagingLink)
                .FirstOrDefault(patient => patient.PatientId == patientId);
            return View("PatientInfo", patient);
        }

        [HttpPost("{patientId}/delete")]
        public IActionResult PatientDelete(int patientId)
        {
            Patient deletedPatient = _context.Patients
                .Include(p => p.MessagingLink)
                .SingleOrDefault(patient => patient.PatientId == patientId);
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