using System.Threading.Tasks;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Infrastructure;

namespace PatientPortal.Services
{
    public class StaffRegistrationService : IStaffRegistrationService
    {
        private readonly IAuthService _authService;
        private readonly PatientPortalContext _context;

        public StaffRegistrationService(
            IAuthService authService,
            PatientPortalContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Staff> RegisterStaffAsync(StaffFormView staffFormView)
        {
            // Create Identity user account
            var createUserResult = await _authService.CreateUserAsync(
                staffFormView.StaffUsername,
                staffFormView.Password
            );
            var identityUser = createUserResult.Value;

            // Create Staff record linked to user
            var staff = new Staff
            {
                User = identityUser,
                FirstName = staffFormView.FirstName,
                LastName = staffFormView.LastName,
                Role = staffFormView.Role,
                IsAdmin = false,
                MessagingLink = new MessagingLink()
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return staff;
        }
    }
}
