using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitViewService : IDisposable
    {
        VisitFormView? ReturnVisitFormView(int patientId);
    }
}