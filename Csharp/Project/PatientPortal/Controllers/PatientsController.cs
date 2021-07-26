using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Controllers
{
    [Route("/provider")]
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
        [HttpGet("patients")]
        public IActionResult PatientManager()
        {
            List<Patient> allPatients = _context.Patients.ToList();
            return View("PatientManager", allPatients);
        }

        [HttpGet("patients/add")]
        public IActionResult PatientAdd()
        {
            return View("PatientForm");
        }

        [HttpPost("patients/add")]
        public IActionResult PatientCreate(Patient newPatient)
        {
            
            if(ModelState.IsValid)
            {
                if(!_context.Patients.Any(patient => patient.Last4SSN == newPatient.Last4SSN 
                    && patient.DOB == newPatient.DOB 
                    && patient.FirstName == newPatient.FirstName 
                    && patient.LastName == newPatient.LastName))
                {
                    _context.Patients.Add(newPatient);
                    _context.SaveChanges();

                    return RedirectToAction("PatientManager", "Patients");
                }
            
                else
                {
                    ModelState.AddModelError("Last4SSN","A patient already exists with these criteria");
                }
            }
            return View("PatientForm");
        }
        
        [HttpGet("patients/{patientId}")]
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

        [HttpPost("patients/{patientId}/delete")]
        public IActionResult PatientDelete(int patientId)
        {
            Patient deletedPatient = _context.Patients.SingleOrDefault(patient => patient.PatientId == patientId);
            if(deletedPatient != null)
            {
                _context.Patients.Remove(deletedPatient);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientManager", "Patients");
        }
        
        //======================Medical Team================================
        [HttpPost("patients/{patientId}/join")]
        public IActionResult MedicalTeamJoin(int patientId)
        {
            PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);

            if(oldLink == null)
            {
                PatientStaffConnection newLink = new PatientStaffConnection()
                {
                    PatientId = patientId,
                    StaffId = (int)uuid
                };
                _context.PatientStaffConnections.Add(newLink);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
        [HttpPost("patients/{patientId}/leave")]
        public IActionResult MedicalTeamLeave(int patientId)
        {
            PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);
            
            if(oldLink != null)
            {
                _context.PatientStaffConnections.Remove(oldLink);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}