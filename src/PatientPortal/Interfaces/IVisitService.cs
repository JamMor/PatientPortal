using System;
using System.Collections.Generic;
using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface IVisitService : IDisposable
    {

        void CreateVisit(int patientId, int staffId, VisitDTO formData);
        void DeleteVisit(int visitId);
    }
}