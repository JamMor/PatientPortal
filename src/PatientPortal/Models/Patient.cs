using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "is required.")]
        [MinLength(2, ErrorMessage = "must be at least 2 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [PastDate]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Must be 4 digits.")]
        [Display(Name = "Last four digits of SSN")]
        public string Last4SSN { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string FullName()
        {
            return FirstName + " " + LastName;
        }

        [NotMapped]
        public int Age 
        {
            get 
            {
                DateTime now = DateTime.Now;
                if(now.DayOfYear >= DOB.DayOfYear)
                {
                    return now.Year - DOB.Year;
                }
                else
                {
                    return now.Year - DOB.Year - 1;
                }
            }
        }

        //Relationship Properties=============

        public List<PatientStaffConnection> MedicalTeam { get; set; }
        public List<HealthIssue> HealthIssues { get; set; }
        public List<TestResult> Tests { get; set; }
        public List<Visit> Visits { get; set; }
        public Address Address { get; set; }
        public MessagingLink MessagingLink { get; set; } = new MessagingLink();
    }
}