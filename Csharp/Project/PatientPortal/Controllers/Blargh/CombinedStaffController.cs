// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using PatientPortal.Models;

// namespace PatientPortal.Controllers
// {
//     [Route("/staff")]
//     public class CombinedStaffController : Controller
//     {
//         private int? uuid
//         {
//             get
//             {
//                 return HttpContext.Session.GetInt32("UserId");
//             }
//         }
//         private bool IsLoggedIn
//         {
//             get
//             {
//                 return uuid != null;
//             }
//         }

//         private PatientPortalContext _context;
//         public CombinedStaffController(PatientPortalContext context)
//         {
//             _context = context;
//         }

// //==============Staff Manager==============================
//         [HttpGet("admin")]
//         public IActionResult StaffManager()
//         {
//             List<Staff> allStaff = _context.Staff
//                 .Include(staff => staff.Patients)
//                 .ToList();
//             return View("StaffManager", allStaff);
//         }

//         [HttpGet("add")]
//         public IActionResult StaffAdd()
//         {
//             return View("StaffForm");
//         }

//         [HttpPost("add")]
//         public IActionResult StaffCreate(Staff newStaff)
//         {
//             if(!_context.Staff.Any(staff => staff.StaffUsername == newStaff.StaffUsername))
//             {
//                 if(ModelState.IsValid)
//                 {
//                     PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
//                     newStaff.Password = hasher.HashPassword(newStaff, newStaff.Password);
//                     newStaff.IsAdmin = false;

//                     _context.Staff.Add(newStaff);
//                     _context.SaveChanges();

//                     return RedirectToAction("StaffManager", "Staff");
//                 }
//             }
//             else
//             {
//                 ModelState.AddModelError("StaffUsername","This username is already taken.");
//             }
//             return View("StaffForm");
//         }

//         [HttpGet("{staffId}")]
//         public IActionResult StaffInfo(int staffId)
//         {
//             Staff staffmember = _context.Staff
//                 .Include(staff => staff.Patients)
//                 .FirstOrDefault(staff => staff.StaffId == staffId);
//             return View("StaffInfo", staffmember);
//         }
    

//         [HttpPost("{staffId}/delete")]
//         public IActionResult StaffDelete(int staffId)
//         {
//             Staff deletedStaff = _context.Staff.SingleOrDefault(staff => staff.StaffId == staffId);
//             if(deletedStaff != null)
//             {
//                 _context.Staff.Remove(deletedStaff);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("StaffManager", "Staff");
//         }

// //===========================Patient Manager==============================
//         [HttpGet("patients")]
//         public IActionResult PatientManager()
//         {
//             List<Patient> allPatients = _context.Patients.ToList();
//             return View("PatientManager", allPatients);
//         }

//         [HttpGet("patients/add")]
//         public IActionResult PatientAdd()
//         {
//             return View("PatientForm");
//         }

//         [HttpPost("patients/add")]
//         public IActionResult PatientCreate(Patient newPatient)
//         {
            
//             if(ModelState.IsValid)
//             {
//                 if(!_context.Patients.Any(patient => patient.Last4SSN == newPatient.Last4SSN 
//                     && patient.DOB == newPatient.DOB 
//                     && patient.FirstName == newPatient.FirstName 
//                     && patient.LastName == newPatient.LastName))
//                 {
//                     _context.Patients.Add(newPatient);
//                     _context.SaveChanges();

//                     return RedirectToAction("PatientManager", "Staff");
//                 }
            
//                 else
//                 {
//                     ModelState.AddModelError("Last4SSN","A patient already exists with these criteria");
//                 }
//             }
//             return View("PatientForm");
//         }
        
//         [HttpGet("patients/{patientId}")]
//         public IActionResult PatientInfo(int patientId)
//         {
//             Patient patient = _context.Patients
//                 .Include(patient => patient.HealthIssues)
//                 .ThenInclude(issue => issue.AssociatedTestResults)
//                 .Include(patient => patient.HealthIssues)
//                 .ThenInclude(issue => issue.AssociatedVisits)
//                 .Include(patient => patient.Visits)
//                 .ThenInclude(team => team.Staff)
//                 .Include(patient => patient.Tests)
//                 .ThenInclude(team => team.Staff)
//                 .Include(patient => patient.MedicalTeam)
//                 .ThenInclude(team => team.Staff)
//                 .FirstOrDefault(patient => patient.PatientId == patientId);
//             return View("PatientInfo", patient);
//         }

//         [HttpPost("patients/{patientId}/delete")]
//         public IActionResult PatientDelete(int patientId)
//         {
//             Patient deletedPatient = _context.Patients.SingleOrDefault(patient => patient.PatientId == patientId);
//             if(deletedPatient != null)
//             {
//                 _context.Patients.Remove(deletedPatient);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientManager", "Staff");
//         }
        
//         //======================Medical Team================================
//         [HttpPost("patients/{patientId}/join")]
//         public IActionResult MedicalTeamJoin(int patientId)
//         {
//             PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);

//             if(oldLink == null)
//             {
//                 PatientStaffConnection newLink = new PatientStaffConnection()
//                 {
//                     PatientId = patientId,
//                     StaffId = (int)uuid
//                 };
//                 _context.PatientStaffConnections.Add(newLink);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//         }
//         [HttpPost("patients/{patientId}/leave")]
//         public IActionResult MedicalTeamLeave(int patientId)
//         {
//             PatientStaffConnection oldLink = _context.PatientStaffConnections.FirstOrDefault(link => link.PatientId == patientId && link.StaffId == (int)uuid);
            
//             if(oldLink != null)
//             {
//                 _context.PatientStaffConnections.Remove(oldLink);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//         }

//         //=====================Create HealthIssue===========================
//         [HttpGet("patients/{patientId}/issue")]
//         public IActionResult HealthIssueAdd(int patientId)
//         {
//             ViewBag.patientId = patientId;
//             return View("HealthIssueForm");
//         }
        
//         [HttpPost("patients/{patientId}/issue")]
//         public IActionResult HealthIssueCreate(int patientId, HealthIssue newIssue)
//         {
//             if(ModelState.IsValid)
//             {
//                 newIssue.PatientId = patientId;
//                 _context.HealthIssues.Add(newIssue);
//                 _context.SaveChanges();
//                 return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//             }
//             return View("HealthIssueForm");
//         }
        

//         [HttpPost("patients/{patientId}/issue/{issueId}/delete")]
//         public IActionResult IssueDelete(int patientId, int issueId)
//         {
//             HealthIssue deletedHealthIssue = _context.HealthIssues.SingleOrDefault(issue => issue.HealthIssueId == issueId);
//             if(deletedHealthIssue != null)
//             {
//                 _context.HealthIssues.Remove(deletedHealthIssue);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//         }

//         //=========================Create Visit=============================
//         [HttpGet("patients/{patientId}/visit")]
//         public IActionResult VisitAdd(int patientId)
//         {
//             ViewBag.patientId = patientId;
//             ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
//             return View("VisitForm");
//         }

//         [HttpPost("patients/{patientId}/visit")]
//         public IActionResult VisitCreate(int patientId, Visit newVisit, List<int> issues)
//         {
//             if(ModelState.IsValid)
//             {
//                 newVisit.PatientId = patientId;
//                 newVisit.StaffId = (int)uuid;
//                 _context.Visits.Add(newVisit);
//                 _context.SaveChanges();

//                 foreach (int issueId in issues)
//                 {
//                     VisitHealthIssueAssociation newAssociation = new VisitHealthIssueAssociation()
//                     {
//                         VisitId = newVisit.VisitId,
//                         HealthIssueId = issueId
//                     };
//                     _context.VisitHealthIssueAssociations.Add(newAssociation);
//                     _context.SaveChanges();
//                 }
//                 return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//             }
//             return View("VisitForm");
//         }
        
//         [HttpPost("patients/{patientId}/visit/{visitId}/delete")]
//         public IActionResult VisitDelete(int patientId, int visitId)
//         {
//             Visit deletedVisit = _context.Visits.SingleOrDefault(visit => visit.VisitId == visitId);
//             if(deletedVisit != null)
//             {
//                 _context.Visits.Remove(deletedVisit);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//         }

//         //======================Create TestResult===========================
//         [HttpGet("patients/{patientId}/test")]
//         public IActionResult TestResultAdd(int patientId)
//         {
//             ViewBag.patientId = patientId;
//             ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
//             return View("TestResultForm");
//         }

//         [HttpPost("patients/{patientId}/test")]
//         public IActionResult TestResultCreate(int patientId, TestResult newTest, List<int> issues)
//         {
//             if(ModelState.IsValid)
//             {
//                 newTest.PatientId = patientId;
//                 newTest.StaffId = (int)uuid;
//                 _context.TestResults.Add(newTest);
//                 _context.SaveChanges();

//                 foreach (int issueId in issues)
//                 {
//                     TestHealthIssueAssociation newAssociation = new TestHealthIssueAssociation()
//                     {
//                         TestResultId = newTest.TestResultId,
//                         HealthIssueId = issueId
//                     };
//                     _context.TestHealthIssueAssociations.Add(newAssociation);
//                     _context.SaveChanges();
//                 }
//                 return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//             }
//             return View("VisitForm");
//         }
    
//         [HttpPost("patients/{patientId}/test/{testId}/delete")]
//         public IActionResult TestResultDelete(int patientId, int testId)
//         {
//             TestResult deletedTestResult = _context.TestResults.SingleOrDefault(issue => issue.TestResultId == testId);
//             if(deletedTestResult != null)
//             {
//                 _context.TestResults.Remove(deletedTestResult);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("PatientInfo", "Staff", new {patientId = patientId});
//         }
//     }
// }