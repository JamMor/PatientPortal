//using system not needed once console.writeline removed...
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class MessagingLink
    {
        [Key]
        public int MessagingLinkId { get; set; }

        public int? StaffId { get; set; }

        [OneUser]
        public int? PatientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<Unread> UnreadMessages { get; set; }
    }
    public class OneUser : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var messenger = (MessagingLink)validationContext.ObjectInstance;
            if ((messenger.StaffId == null && messenger.PatientId == null) || (messenger.StaffId != null && messenger.PatientId != null))
            {
                return new ValidationResult("Error with assigning users to MessengerLink.");
            }
            else
            {
                Console.WriteLine("Only one assigned User!");
                if (messenger.StaffId != null)
                {
                    Console.WriteLine("It's a staffmember!");
                }
                if (messenger.PatientId != null)
                {
                    Console.WriteLine("It's a patient!");
                }
                return ValidationResult.Success;
            }
        }
    }
}