
using System;
using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class StaffInfoViewModel
    {
        public int StaffId { get; set; }
        public int MessagingLinkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public int PatientCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string FullName()
        {
            return FirstName + " " + LastName;
        }
    }
}