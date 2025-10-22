// Form Input Model
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class ReplyView
    {
        public int ConversationId { get; set; }

        [Display(Name = "Reply: ")]
        public string? MessageText { get; set; }

        public string? ReturnRoute { get; set; }
    }
}
