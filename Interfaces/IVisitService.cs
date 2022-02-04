using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitService : IDisposable
    {

        void CreateVisit(int patientId, int staffId, VisitFormView formData);
        void DeleteVisit(int visitId);
    }
}