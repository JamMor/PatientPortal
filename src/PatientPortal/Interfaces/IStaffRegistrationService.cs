using System.Threading.Tasks;
using PatientPortal.Models;
using PatientPortal.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace PatientPortal.Interfaces
{
    public interface IStaffRegistrationService
    {
        Task<ExtendedIdentityResult<Staff>> RegisterStaffAsync(StaffFormView staffFormView);
        Task<IdentityResult> DeleteStaffAsync(int staffId);
    }
}
