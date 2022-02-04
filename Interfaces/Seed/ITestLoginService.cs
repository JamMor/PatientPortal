using System;
using System.Collections.Generic;
using Bogus;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestLoginService : IDisposable
    {
        List<TestLoginViewModel> GetAllStaff();
        LoginStaffDTO CreateAdmin();
        LoginStaffDTO LoginStaffById(int staffId);
    }
}