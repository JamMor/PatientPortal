using System;
using System.Collections.Generic;
using System.Linq;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface ILoginService : IDisposable
    {

        public Staff DoesStaffUserExist(string username);
        public bool VerifyStaffPassword(LoginStaff loginStaff, Staff savedStaff);
        public LoginStaffDTO AttemptStaffLogin(LoginStaff loginStaff);
    }
}