using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PatientPortal.Interfaces;
using PatientPortal.Infrastructure;

namespace PatientPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ExtendedIdentityResult<IdentityUser>> CreateUserAsync(string username, string password)
        {
            var user = new IdentityUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            return new ExtendedIdentityResult<IdentityUser>(result, user);
        }

        public async Task<SignInResult> SignInAsync(string username, string password)
        {
            return await _signInManager.PasswordSignInAsync(
                username,
                password,
                isPersistent: false,
                lockoutOnFailure: true
            );
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
