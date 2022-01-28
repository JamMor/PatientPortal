using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IPatientStaffConnectionService : IDisposable
    {
        void AddStaffToPatientTeam(int patientId, int staffId);
        void RemoveStaffFromPatientTeam(int patientId, int staffId);
    }
}