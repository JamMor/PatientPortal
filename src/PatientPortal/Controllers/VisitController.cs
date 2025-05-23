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

        private IPatientViewService _patientViewService;
        private IVisitService _visitService;
        private IVisitViewService _visitViewService;
        public VisitController(IPatientViewService patientViewService, IVisitService visitService, IVisitViewService visitViewService)
        {
            _patientViewService = patientViewService;
            _visitService = visitService;
            _visitViewService = visitViewService;
        }

//=========================Create Visit=============================
        [HttpGet("")]
        public IActionResult VisitAdd(int patientId)
        {
            VisitFormView viewModel = _visitViewService.ReturnVisitFormView(patientId);
            
            return View("VisitForm", viewModel);
        }

        [HttpPost("")]
        public IActionResult VisitCreate(int patientId, VisitFormView formData)
        {
            if(ModelState.IsValid)
            {
                _visitService.CreateVisit(patientId, (int)uuid, formData);
                
                return RedirectToAction("PatientInfo", "Patients", new {patientId = patientId});
            }
            
            formData.Patient = _patientViewService.GetPatientInfoHeader(patientId);
            
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