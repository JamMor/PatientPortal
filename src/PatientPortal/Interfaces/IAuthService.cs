using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PatientPortal.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityUser> CreateUserAsync(string username, string password);
        Task<SignInResult> SignInAsync(string username, string password);
        Task SignOutAsync();
    }
}
