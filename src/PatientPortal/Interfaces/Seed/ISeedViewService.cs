using System;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ISeedViewService : IDisposable
    {
        SeedFormView ReturnSeedFormView();
        
        void SeedNStaff(int staffAmount);
        void SeedNPatients(int patientAmount);
    }
}