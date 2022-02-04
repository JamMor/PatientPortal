using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class TestLoginService : ITestLoginService
    {
        private PatientPortalContext _context;

        public TestLoginService(PatientPortalContext context)
        {
            _context = context;
        }

        public LoginStaffDTO CreateAdmin()
        {
            Staff newAdmin = new Staff()
            {
                IsAdmin = true,
                FirstName = "Jean-Luc",
                LastName = "Picard",
                Role = "Admin",
                StaffUsername = "JPicardNumber1",
                Password = "password0$",
                MessagingLink = new MessagingLink()
            };
            PasswordHasher<Staff> hasher = new PasswordHasher<Staff>();
            newAdmin.Password = hasher.HashPassword(newAdmin, newAdmin.Password);

            _context.Staff.Add(newAdmin);
            _context.SaveChanges();

            return new LoginStaffDTO()
                {
                    StaffId = newAdmin.StaffId,
                    MessagingLinkId = newAdmin.MessagingLink.MessagingLinkId,
                    IsAdmin = newAdmin.IsAdmin,
                    FullName = newAdmin.FullName(),
                    Role = newAdmin.Role
                };

        }
        
        public LoginStaffDTO LoginStaffById(int staffId)
        {
            Staff staffmember = _context.Staff
                .Include(staff => staff.MessagingLink)
                .FirstOrDefault(s => s.StaffId == staffId);

            return new LoginStaffDTO()
                {
                    StaffId = staffmember.StaffId,
                    MessagingLinkId = staffmember.MessagingLink.MessagingLinkId,
                    IsAdmin = staffmember.IsAdmin,
                    FullName = staffmember.FullName(),
                    Role = staffmember.Role
                };
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