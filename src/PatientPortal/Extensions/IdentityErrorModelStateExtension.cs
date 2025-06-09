using Microsoft.AspNetCore.Mvc.ModelBinding;
using PatientPortal.Infrastructure;

namespace PatientPortal.Extensions
{
    public static class IdentityErrorModelStateExtension
    {
        public static void AddErrorDictionaryToModelState<T>(
        this ExtendedIdentityResult<T> result,
        ModelStateDictionary modelState,
        string usernameField = "",
        string passwordField = "",
        string confirmPasswordField = ""
    )
        {
            var errorDict = result.MapIdentityErrorsToFields(
                usernameField,
                passwordField,
                confirmPasswordField
            );

            foreach (var (fieldName, errorMessages) in errorDict)
            {
                foreach (var errorMessage in errorMessages)
                {
                    modelState.AddModelError(fieldName, errorMessage);
                }
            }
        }
    }
}