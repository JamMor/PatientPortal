using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class LoginStaff
    {    
        [Required(ErrorMessage = "is required.")]
        [Display(Name = "Username")]
        public string StaffUsername { get; set; }

        [Required(ErrorMessage = "is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }
    }
}