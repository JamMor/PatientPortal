#nullable enable
// Form Input Model
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatientPortal.Shared.Validation;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientSearch
    {
        [Display(Name = "Patient ID #")]
        public int? SearchPatientId { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? SearchFirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? SearchLastName { get; set; }

        [Display(Name = "SSN")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Must be 4 digits.")]
        public string? SearchSSN { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [PastDate]
        public DateTime? SearchBirthdate { get; set; }

        [Display(Name = "Only Show Patients Under My Care")]
        public bool SearchPatientsUnderCare { get; set; }
    }
}
