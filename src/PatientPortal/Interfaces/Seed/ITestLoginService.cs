using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientPortal.Infrastructure;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ITestLoginService : IDisposable
    {
        List<TestLoginViewModel> GetAllStaff();
        Task<ExtendedIdentityResult<Staff>> CreateAdmin();
        Task LoginStaffById(int staffId);
    }
}