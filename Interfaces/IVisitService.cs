using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitService : IDisposable
    {

        // Visit GetVisitbyId(int visitId);
        void CreateVisit(int patientId, int staffId, Visit newVisit, List<int> issues);
        // void UpdateVisit(Visit visitInfo);
        void DeleteVisit(int visitId);
    }
}