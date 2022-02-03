using System;
using System.Collections.Generic;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ISeedAndTestingService : IDisposable
    {

        void CreateInitialAdmin();
        void LoginStaffById(int staffId);
        void GetNumberOfStaff();
        void GetNumberOfPatients();

        void SeedDatabase(string option, SeedTestView seedAmount);

        void Save();
    }
}