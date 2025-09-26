using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class ConversationSearch
    {
        
        public bool IsPatientInbox { get; set; }

        [Display(Name ="Only Show Unread")]
        public bool OnlyUnread { get; set; } = false;
    }
}