// Form Input Model
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    [NotMapped]
    public class NewConversationFormView
    {
        public List<Recipient> Recipients { get; set; } = [];
        public Recipient? PatientRecipient { get; set; }
        public string? Subject { get; set; }

        [Required]
        public string? MessageText { get; set; }
        public bool WithPatient
        {
            get
            {
                if (PatientRecipient == null)
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
        public required string Name { get; set; }
        public required string Role { get; set; }
        public bool Selected { get; set; } = false;
    }

    public static class NewConversationFormViewExtensions
    {
        public static NewConversationFormView ApplyInput(this NewConversationFormView form, NewConversationFormInput input)
        {
            form.Subject = input.Subject;
            form.MessageText = input.MessageText;
            foreach (var recipient in form.Recipients)
            {
                var selection = input.Recipients
                    .FirstOrDefault(r => r.LinkId == recipient.LinkId);
                if (selection != null)
                {
                    recipient.Selected = selection.Selected;
                }
            }
            if (form.PatientRecipient != null && input.PatientRecipient != null)
            {
                form.PatientRecipient.Selected = input.PatientRecipient.Selected;
            }
            return form;
        }
    }
}
