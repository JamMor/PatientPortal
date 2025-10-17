using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitViewService : IDisposable
    {
        VisitFormView GetNewVisitForm(int patientId);
    }
}