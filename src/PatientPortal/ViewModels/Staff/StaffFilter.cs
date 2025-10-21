using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffFilter
    {
        [Display(Name = "Staff ID #")]
        public int? StaffId { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression("^[a-zA-Z\\s]*$", ErrorMessage = "May only contain letters.")]
        public string? LastName { get; set; }

        [Display(Name = "Role")]
        public string? Role { get; set; }

        public Dictionary<string, string> ToRouteDict()
        {
            var dict = new Dictionary<string, string>();
            if (StaffId.HasValue)
                dict["StaffId"] = StaffId.Value.ToString();
            if (!string.IsNullOrEmpty(FirstName))
                dict["FirstName"] = FirstName;
            if (!string.IsNullOrEmpty(LastName))
                dict["LastName"] = LastName;
            if (!string.IsNullOrEmpty(Role))
                dict["Role"] = Role;
            return dict;
        }
    }
}
