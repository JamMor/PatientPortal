using System;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Shared.Validation

{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime date)
            {
                return ValidationResult.Success;
            }

            return date > DateTime.Today
                ? new ValidationResult("Date must be in the past.")
                : ValidationResult.Success;
        }
    }
}