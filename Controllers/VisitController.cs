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
    [Route("/provider/patients/{patientId}/visit")]
    public class VisitController : Controller
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
        private IVisitService _visitService;
        public VisitController(PatientPortalContext context, IVisitService visitService)
        {
            _context = context;
            _visitService = visitService;
        }

//=========================Create Visit=============================
        [HttpGet("")]
        public IActionResult VisitAdd(int patientId)
        {
            ViewBag.patientId = patientId;
            ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
            return View("VisitForm");
        }

        [HttpPost("")]
        public IActionResult VisitCreate(int patientId, Visit newVisit, List<int> issues)
        {
            if(ModelState.IsValid)
            {
                _visitService.CreateVisit(patientId, (int)uuid, newVisit, issues);
                
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            ViewBag.patientId = patientId;
            ViewBag.HealthIssues = _context.HealthIssues.Where(issue => issue.PatientId == patientId).ToList();
            return View("VisitForm");
        }
        
        [HttpPost("{visitId}/delete")]
        public IActionResult VisitDelete(int patientId, int visitId)
        {
            _visitService.DeleteVisit(visitId);
            
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}