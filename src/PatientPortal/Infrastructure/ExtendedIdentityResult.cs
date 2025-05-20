using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace PatientPortal.Infrastructure
{
    /// <summary>
    /// Wraps an IdentityResult with an optional associated value (e.g., the created user).
    /// Provides mapping Identity errors to form field names for validation.
    /// </summary>
    public class ExtendedIdentityResult<T>
    {
        public IdentityResult IdentityResult { get; }
        public T? Value { get; }

        public bool Succeeded => IdentityResult.Succeeded;

        public ExtendedIdentityResult(IdentityResult result, T? value = default)
        {
            IdentityResult = result;
            Value = value;
        }

        public Dictionary<string, List<string>> MapIdentityErrorsToFields(
            string usernameField = "",
            string passwordField = "",
            string confirmPasswordField = ""
        )
        {
            return IdentityResult.Errors
                .GroupBy(error => error.Code switch
                {
                    string c when c.Contains("UserName") => usernameField,
                    string c when c.Contains("Mismatch") => confirmPasswordField,
                    string c when c.Contains("Password") && !c.Contains("Mismatch") => passwordField,
                    _ => "",
                })
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());
        }

        public static ExtendedIdentityResult<T> Success(T value)
        {
            return new ExtendedIdentityResult<T>(IdentityResult.Success, value);
        }

        public static ExtendedIdentityResult<T> Failure(IdentityResult identityResult)
        {
            return new ExtendedIdentityResult<T>(identityResult, default);
        }
    }

}