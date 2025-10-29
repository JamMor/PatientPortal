// Form Input Model
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class InboxSearchForm
    {
        [Display(Name = "Only Show Unread")]
        public bool OnlyUnread { get; set; }
        public int ResultsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; } = 1;
    }
}
