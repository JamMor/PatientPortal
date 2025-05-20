using System.Threading.Tasks;
using PatientPortal.Models;
using PatientPortal.Infrastructure;

namespace PatientPortal.Interfaces
{
    public interface IStaffRegistrationService
    {
        Task<ExtendedIdentityResult<Staff>> RegisterStaffAsync(StaffFormView staffFormView);
    }
}
