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
    [Route("/provider/patients/{patientId}/visit")]
    public class VisitController : Controller
    {
        private int? staffId => User.GetStaffId();

        private IVisitService _visitService;
        private IVisitViewService _visitViewService;
        public VisitController(IVisitService visitService, IVisitViewService visitViewService)
        {
            _visitService = visitService;
            _visitViewService = visitViewService;
        }

//=========================Create Visit=============================
        [HttpGet("")]
        public IActionResult VisitAdd(int patientId)
        {
            VisitForm? form = _visitViewService.ReturnVisitForm(patientId);
            if(form == null)
            {
                return NotFound();
            }

            return View("VisitForm", form);
        }

        [HttpPost("")]
        public IActionResult VisitCreate(int patientId, VisitForm formData)
        {
            if(!staffId.HasValue)
            {
                return StatusCode(500, "Unable to retrieve staff information. Please ensure you are logged in and try again.");
            }
            
            if(ModelState.IsValid)
            {
                try
                {
                    var visitDTO = formData.ToVisitDTO();
                    _visitService.CreateVisit(patientId, (int)staffId, visitDTO);
                    
                    return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
                }
                catch
                {
                    // Log the exception (not implemented here)
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while creating the visit.");
                }
            }

            return View("VisitForm", formData);
        }
        
        [HttpPost("{visitId}/delete")]
        public IActionResult VisitDelete(int patientId, int visitId)
        {
            _visitService.DeleteVisit(visitId);
            
            return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
        }
    }
}