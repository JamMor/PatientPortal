#nullable enable
using System.Threading.Tasks;
using PatientPortal.Models;
using PatientPortal.Infrastructure;
using Microsoft.AspNetCore.Identity;
using PatientPortal.DTOs;

namespace PatientPortal.Interfaces
{
    public interface IStaffRegistrationService
    {
        Task<ExtendedIdentityResult<Staff>> RegisterStaffAsync(AccountDTO accountDTO, StaffDTO staffDTO);
        Task<IdentityResult> DeleteStaffAsync(int staffId);
    }
}
