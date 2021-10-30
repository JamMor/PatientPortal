using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class NewConversationFormView
    {
        public List<Recipient> Recipients { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }

        public bool WithPatient { get; set; } = false;

    }

    [NotMapped]
    public class Recipient
    {
        public int LinkId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public bool Selected { get; set; } = false;

    }
}