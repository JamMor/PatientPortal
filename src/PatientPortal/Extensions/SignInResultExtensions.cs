using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PatientPortal.Extensions
{
    public static class SignInResultModelStateExtension
    {
        public static void AddErrorToModelState(
        this SignInResult result,
        ModelStateDictionary modelState
    )
        {
            if (result.IsLockedOut)
            {
                modelState.AddModelError(string.Empty, "Account is locked due to multiple failed login attempts. Please try again later.");
            }
            if (result.IsNotAllowed) {
                modelState.AddModelError(string.Empty, "Login is not allowed for this account.");
            }
            if (result.RequiresTwoFactor) {
                modelState.AddModelError(string.Empty, "Two-factor authentication is required.");
            }
            else
            {
                modelState.AddModelError(string.Empty, "Invalid credentials.");
            }
        }
    }
}