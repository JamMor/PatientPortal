using System;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitViewService : IDisposable
    {
        VisitForm? ReturnVisitForm(int patientId);
    }
}