using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PatientPortal.Infrastructure;

namespace PatientPortal.Interfaces
{
    public interface IAuthService
    {
        Task<ExtendedIdentityResult<IdentityUser>> CreateUserAsync(string username, string password);
        Task<IdentityResult> DeleteUserAsync(IdentityUser user);
        Task<SignInResult> SignInAsync(string username, string password);
        Task SignOutAsync();
    }
}
