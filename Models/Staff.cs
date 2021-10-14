using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        public bool IsAdmin { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "is required.")]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(10, ErrorMessage = "must be at least 10 characters.")]
        [Display(Name = "Username")]
        public string StaffUsername { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(10, ErrorMessage = "must be at least 10 characters.")]
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

        public int MessagingLinkId { get; set; }

        public string FullName()
        {
            return FirstName + " " + LastName;
        }

        //Relationship Properties=============

        public List<PatientStaffConnection> Patients { get; set; }
        public List<Visit> Visits { get; set; }
        public List<TestResult> TestsOrdered { get; set; }
        public MessagingLink MessagingLink { get; set; } = new MessagingLink();
    }
}