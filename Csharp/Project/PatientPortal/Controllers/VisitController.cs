using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public VisitController(PatientPortalContext context)
        {
            _context = context;
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
                newVisit.PatientId = patientId;
                newVisit.StaffId = (int)uuid;
                _context.Visits.Add(newVisit);
                _context.SaveChanges();

                foreach (int issueId in issues)
                {
                    VisitHealthIssueAssociation newAssociation = new VisitHealthIssueAssociation()
                    {
                        VisitId = newVisit.VisitId,
                        HealthIssueId = issueId
                    };
                    _context.VisitHealthIssueAssociations.Add(newAssociation);
                    _context.SaveChanges();
                }
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            return View("VisitForm");
        }
        
        [HttpPost("{visitId}/delete")]
        public IActionResult VisitDelete(int patientId, int visitId)
        {
            Visit deletedVisit = _context.Visits.SingleOrDefault(visit => visit.VisitId == visitId);
            if(deletedVisit != null)
            {
                _context.Visits.Remove(deletedVisit);
                _context.SaveChanges();
            }
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}