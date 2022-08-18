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
        public Recipient PatientRecipient { get; set; }
        public string Subject { get; set; }
        [Required]
        public string MessageText { get; set; }
        public bool WithPatient 
        {
            get
            {
                if(PatientRecipient == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
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