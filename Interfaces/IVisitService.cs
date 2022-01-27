using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IVisitService : IDisposable
    {

        Visit GetVisitbyId(int visitId);
        void CreateVisit(Visit visitInfo);
        void UpdateVisit(Visit visitInfo);
        void DeleteVisit(int visitId);

        void Save();
    }
}