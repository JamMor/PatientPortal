using System.Threading.Tasks;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
                return ExtendedIdentityResult<Staff>.Failure(createUserResult.IdentityResult);
            }

            Staff staff = _staffService.CreateStaff(staffFormView, createUserResult.Value);

            return ExtendedIdentityResult<Staff>.Success(staff);
        }

        public async Task<IdentityResult> DeleteStaffAsync(int staffId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Staff staffToDelete = await _staffService.GetStaffbyId(staffId)
                    .Include(s => s.User)
                    .SingleOrDefaultAsync();

                // If the staff member doesn't exist, return failure
                if (staffToDelete == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Staff member not found." });
                }
                
                // If the staff member has an associated user account, delete it
                if (staffToDelete.User != null)
                {
                    var result = await _authService.DeleteUserAsync(staffToDelete.User);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }

                // Delete the staff member
                _staffService.DeleteStaff(staffId);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                System.Console.WriteLine($"Error deleting staff with ID: {staffId} in StaffReg service. Exception: {ex}");
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while deleting the staff member." });
            }

            return IdentityResult.Success;
        }
    }
}
