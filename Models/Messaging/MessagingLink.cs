//using system not needed once console.writeline removed...
//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class MessagingLink
    {
        [Key]
        public int MessagingLinkId { get; set; }

        [ForeignKey("Staff")]
        public int? StaffId { get; set; }

        [OneUser]
        [ForeignKey("Patient")]
        public int? PatientId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Staff Staff { get; set; }
        public Patient Patient { get; set; }
        public List<Unread> UnreadMessages { get; set; }
        public List<ConversationParticipant> ParticipatingConversations { get; set; }

        [NotMapped]
        public string UserType
        {
            get => StaffId == null ? "Patient" : "Staff";
        }
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