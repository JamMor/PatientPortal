using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models;

public class NewConversationFormInput
{
    public string? Subject { get; set; }

    [Required]
    public string? MessageText { get; set; }

    public bool WithPatient { get; set; }

    public List<RecipientSelection> Recipients { get; set; } = [];

    public RecipientSelection? PatientRecipient { get; set; }

    public class RecipientSelection
    {
        public int LinkId { get; set; }
        public bool Selected { get; set; }
    }
}
