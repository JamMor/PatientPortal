using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class NewMessageFormView
    {
        public List<Recipient> Recipients { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }

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