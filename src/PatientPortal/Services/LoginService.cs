using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    [Obsolete("This service is no longer used. It has been replaced by AuthService and DemoLoginService.")]
    public class LoginService : ILoginService
    {
        private PatientPortalContext _context;
        public LoginService(PatientPortalContext context)
        {
            _context = context;
        }

        public Staff DoesStaffUserExist(string username)
        {
            return _context.Staff
                    .Include(staff => staff.MessagingLink)
                    .FirstOrDefault(staff => staff.StaffUsername == username);
        }
        public bool VerifyStaffPassword(LoginStaff loginStaff, Staff savedStaff)
        {
            PasswordHasher<LoginStaff> hasher = new PasswordHasher<LoginStaff>();
            PasswordVerificationResult passwordVerification = hasher
                .VerifyHashedPassword(loginStaff, savedStaff.Password, loginStaff.LoginPassword);
            return passwordVerification != 0;    
        }

        public LoginStaffDTO AttemptStaffLogin(LoginStaff loginStaff)
        {
            Staff savedStaff = DoesStaffUserExist(loginStaff.StaffUsername);
            if(savedStaff != null)
            {
                if(VerifyStaffPassword(loginStaff, savedStaff))
                {
                    return new LoginStaffDTO()
                    {
                        StaffId = savedStaff.StaffId,
                        MessagingLinkId = savedStaff.MessagingLink.MessagingLinkId,
                        IsAdmin = savedStaff.IsAdmin,
                        FullName = savedStaff.FullName(),
                        Role = savedStaff.Role
                    };
                }
            }
            return null;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _context.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PatientService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}