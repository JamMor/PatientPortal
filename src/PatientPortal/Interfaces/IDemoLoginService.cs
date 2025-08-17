using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientPortal.Infrastructure;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IDemoLoginService : IDisposable
    {
        List<DemoLoginViewModel> GetAllStaff();
        Task<ExtendedIdentityResult<Staff>> CreateAdmin();
        Task<bool> LoginStaffById(int staffId);
    }
}