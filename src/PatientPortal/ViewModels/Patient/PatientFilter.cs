using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PatientPortal.Shared.Validation;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientFilter
    {
        [Display(Name = "Patient ID #")]
        public int? PatientId { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? LastName { get; set; }

        [Display(Name = "SSN")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "Must be 4 digits.")]
        public string? SSN { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [PastDate]
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Only Show Patients Under My Care")]
        public bool OnlyPatientsUnderCare { get; set; }

        public Dictionary<string, string> ToRouteDict()
        {
            var dict = new Dictionary<string, string>();
            if (PatientId.HasValue)
                dict["PatientId"] = PatientId.Value.ToString();
            if (!string.IsNullOrEmpty(FirstName))
                dict["FirstName"] = FirstName;
            if (!string.IsNullOrEmpty(LastName))
                dict["LastName"] = LastName;
            if (!string.IsNullOrEmpty(SSN))
                dict["SSN"] = SSN;
            if (Birthdate.HasValue)
                dict["Birthdate"] = Birthdate.Value.ToString("yyyy-MM-dd");
            if (OnlyPatientsUnderCare)
                dict["OnlyPatientsUnderCare"] = "true";
            return dict;
        }
    }
}
