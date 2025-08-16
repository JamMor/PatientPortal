#nullable enable
// Form Input Model
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffSearch
    {
        [Display(Name = "Staff ID #")]
        public int? SearchStaffId { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? SearchFirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? SearchLastName { get; set; }

        [Display(Name = "Role")]
        public string? SearchRole { get; set; }
    }
}
