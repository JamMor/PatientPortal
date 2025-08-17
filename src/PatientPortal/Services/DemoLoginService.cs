#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PatientPortal.Infrastructure;
using PatientPortal.Interfaces;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class DemoLoginService : IDemoLoginService
    {
        private PatientPortalContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private IAuthService _authService;

        public DemoLoginService(PatientPortalContext context, IAuthService authService, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _authService = authService;
            _signInManager = signInManager;
        }

        public List<DemoLoginViewModel> GetAllStaff()
        {
            return _context.Staff
                .Where(s => s.User != null)
                .OrderBy(s => s.StaffId)
                .Select(s => new DemoLoginViewModel()
                {
                    DemoStaffId = s.StaffId,
                    DemoStaffName = $"{s.FirstName[0]} {s.LastName}",
                    DemoStaffRole = s.Role,
                    IsDemoStaffAdmin = s.IsAdmin
                })
                .ToList();
        }

        public async Task<ExtendedIdentityResult<Staff>> CreateAdmin()
        {
            string adminUsername = "EMcDNumber1";
            string adminPassword = "Password0$";
            
            var result = await _authService.CreateUserAsync(
                adminUsername, 
                adminPassword);

            if (result.Succeeded)
            {
                Staff newAdmin = new Staff()
                {
                    IsAdmin = true,
                    FirstName = "Erin",
                    LastName = "Macdonald",
                    Role = "Admin",
                    StaffUsername = adminUsername,
                    Password = adminPassword,
                    User = result.Value,
                    MessagingLink = new MessagingLink()
                };
                
                _context.Staff.Add(newAdmin);
                _context.SaveChanges();
                
                return ExtendedIdentityResult<Staff>.Success(newAdmin);
            }

            return ExtendedIdentityResult<Staff>.Failure(result.IdentityResult);
        }
        
        public async Task<bool> LoginStaffById(int staffId)
        {
            IdentityUser? user = _context
                .Staff.Where(s => s.StaffId == staffId)
                .Select(s => s.User)
                .FirstOrDefault();

            if (user != null)
            {
                try
                {
                    await _signInManager.SignInAsync(user, false);
                    return true;
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(
                        $"An error occurred during demo one click sign-in: {ex.Message}"
                    );
                    return false;
                }
            }

            return false;
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
        // ~DemoLoginService()
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