using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    //Association created when message sent, and then removed when READ. 
    //No status for read other than lack of association.
    public class Unread
    {
        [Key]
        public int UnreadId { get; set; }
        public int MessagingLinkId { get; set; }
        public int MessageId { get; set; }

        //====================================================================
        public MessagingLink MessagingLink { get; set; }
        public Message Message { get; set; }
    }
}