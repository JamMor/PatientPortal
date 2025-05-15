using System.Threading.Tasks;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffRegistrationService
    {
        Task<Staff> RegisterStaffAsync(StaffFormView staffFormView);
    }
}
