// Form Input Model
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class HealthIssueForm
    {
        [Required]
        [MaxLength(30)]
        [Display(Name = "Short Description")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Long Description")]
        public string? LongDescription { get; set; }
    }
}
