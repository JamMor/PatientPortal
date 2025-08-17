using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitViewService : IDisposable
    {
        VisitFormView? ReturnVisitFormView(int patientId);
    }
}