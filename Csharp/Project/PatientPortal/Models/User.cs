using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage ="must be at least 2 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage ="must be at least 2 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [PastDate]
        [Display(Name ="Date of Birth")]
        public DateTime DOB {get; set; }
        
        [Required]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Must be 4 digits.")]
        [Display(Name = "Last four digits of SSN")]
        public string Last4SSN { get;set; }
        
        [Required(ErrorMessage = "is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [NotMapped]
        [Required(ErrorMessage = "is required.")]
        [Compare("Email")]
        [EmailAddress]
        [Display(Name = "Verify Email")]
        public string VerifyEmail { get; set; }
        
        [Required(ErrorMessage = "is required.")]
        [MinLength(10, ErrorMessage ="must be at least 10 characters.")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$", ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "is required.")]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string FullName()
        {
            return FirstName + " " + LastName;
        }

//Relationship Properties=============

    }

    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // if(value == null)
            // {
            //     return new ValidationResult("Must enter a date.");
            // }
            DateTime today = DateTime.Today;
            if (value != null && (DateTime)value > today)
            {
                return new ValidationResult("Date must be in the past.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}