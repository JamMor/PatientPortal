using System.Threading.Tasks;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Infrastructure;

namespace PatientPortal.Services
{
    public class StaffRegistrationService : IStaffRegistrationService
    {
        private readonly IAuthService _authService;
        private readonly IStaffService _staffService;
        private readonly PatientPortalContext _context;

        public StaffRegistrationService(
            IAuthService authService,
            IStaffService staffService,
            PatientPortalContext context)
        {
            _authService = authService;
            _staffService = staffService;
            _context = context;
        }

        public async Task<ExtendedIdentityResult<Staff>> RegisterStaffAsync(StaffFormView staffFormView)
        {
            // Create Identity user account
            var createUserResult = await _authService.CreateUserAsync(
                staffFormView.StaffUsername,
                staffFormView.Password
            );

            if (!createUserResult.Succeeded)
            {
                return new ExtendedIdentityResult<Staff>(createUserResult.IdentityResult, null);
            }

            Staff staff = _staffService.CreateStaff(staffFormView, createUserResult.Value);

            return new ExtendedIdentityResult<Staff>(createUserResult.IdentityResult, staff);
        }
    }
}
