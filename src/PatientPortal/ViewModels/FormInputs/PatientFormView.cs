// Form Input Model
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PatientPortal.Shared.Validation;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientFormView
    {
        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "is required.")]
        [DataType(DataType.Date)]
        [PastDate]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "is required.")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "must be 4 digits.")]
        [Display(Name = "Last four digits of SSN")]
        public string? Last4SSN { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        public AddressFormView? Address { get; set; }
    }

    public class AddressFormView : IValidatableObject
    {
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        [Display(Name = "ZIP Code")]
        [DataType(DataType.PostalCode)]
        public string? ZipCode { get; set; }

        System.Collections.Generic.IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            string?[] fields = [StreetAddress, City, State, ZipCode];

            if (!fields.All(f => f is null) && fields.Any(f => f is null))
                yield return new ValidationResult("Address must be completely filled out if given.");
        }
    }
}
