using Microsoft.AspNetCore.Mvc;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.ViewComponents
{
    public class PatientInfoHeaderViewComponent : ViewComponent
    {
        private readonly IPatientViewService _patientViewService;

        public PatientInfoHeaderViewComponent(IPatientViewService patientViewService)
        {
            _patientViewService = patientViewService;
        }

        public IViewComponentResult Invoke(int patientId)
        {
            PatientHeaderInfoView? header = _patientViewService.GetPatientInfoHeader(patientId);
            if (header == null)
                return Content(string.Empty);

            return View(header);
        }
    }
}
