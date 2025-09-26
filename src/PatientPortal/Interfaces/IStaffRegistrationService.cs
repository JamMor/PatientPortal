using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PatientPortal.DTOs;
using PatientPortal.Infrastructure;
using PatientPortal.Models;

namespace PatientPortal.Interfaces
{
    public interface IStaffRegistrationService
    {
        Task<ExtendedIdentityResult<Staff>> RegisterStaffAsync(AccountDTO accountDTO, StaffDTO staffDTO);
        Task<IdentityResult> DeleteStaffAsync(int staffId);
    }
}
