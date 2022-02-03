using System;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ISeedViewService : IDisposable
    {
        void SeedNStaff(int staffAmount);
        void SeedNPatients(int patientAmount);
    }
}