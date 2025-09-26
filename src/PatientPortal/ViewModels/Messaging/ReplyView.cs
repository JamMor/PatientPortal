// Form Input Model
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    [NotMapped]
    public class ReplyView
    {
        public int ConversationId { get; set; }

        [Display(Name = "Reply: ")]
        public string? MessageText { get; set; }
    }
}
