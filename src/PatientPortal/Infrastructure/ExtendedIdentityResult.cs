using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace PatientPortal.Infrastructure
{
    public class ExtendedIdentityResult<T>
    {
        public IdentityResult IdentityResult { get; }
        public T? Value { get; }

        public bool Succeeded => IdentityResult.Succeeded;
        public enum ErrorField
        {
            Username,
            Password,
            ConfirmPassword,
            Generic
        }

        public ExtendedIdentityResult(IdentityResult result, T? value)
        {
            IdentityResult = result;
            Value = value;
        }

        public Dictionary<ErrorField, List<string>> MapErrorsToFields()
        {
            return IdentityResult.Errors
                .GroupBy(error => error.Code switch
                {
                    string c when c.Contains("UserName") => ErrorField.Username,
                    string c when c.Contains("Mismatch") => ErrorField.ConfirmPassword,
                    string c when c.Contains("Password") && !c.Contains("Mismatch") => ErrorField.Password,
                    _ => ErrorField.Generic,
                })
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToList());
        }
    }

}