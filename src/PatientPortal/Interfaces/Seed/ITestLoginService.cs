using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestLoginService : IDisposable
    {
        List<TestLoginViewModel> GetAllStaff();
        Task<Staff> CreateAdmin();
        Task LoginStaffById(int staffId);
    }
}