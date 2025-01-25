using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class ConversationSearch
    {
        
        public bool IsPatientInbox { get; set; }

        [Display(Name ="Only Show Unread")]
        public bool OnlyUnread { get; set; } = false;
    }
}