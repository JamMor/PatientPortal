//using system not needed once console.writeline removed...
//
using System;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string MessageText { get; set; }

        public int? StaffId { get; set; }

        [OneSender]
        public int? PatientId { get; set; }

        public int? S2SId { get; set; }

        [OneConversation]
        public int? S2PId { get; set; }

        public StafftoStaffConversation S2SConversation { get; set; }
        public StafftoPatientConversation S2PConversation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class OneSender : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var message = (Message)validationContext.ObjectInstance;
            if ((message.StaffId == null && message.PatientId == null) || (message.StaffId != null && message.PatientId != null))
            {
                return new ValidationResult("Error with user ID of sender.");
            }
            else
            {
                Console.WriteLine("Only one sender!");
                if (message.StaffId != null)
                {
                    Console.WriteLine("It's a staffmember!");
                }
                if (message.PatientId != null)
                {
                    Console.WriteLine("It's a patient!");
                }
                return ValidationResult.Success;
            }
        }
    }
    public class OneConversation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var message = (Message)validationContext.ObjectInstance;
            if ((message.S2SId == null && message.S2PId == null) || (message.S2SId != null && message.S2PId != null))
            {
                return new ValidationResult("Error sending message to conversation.");
            }
            else
            {
                Console.WriteLine("Only one conversation!");
                if (message.S2SId != null)
                {
                    Console.WriteLine("It's a staff to staff!");
                }
                if (message.S2SId != null)
                {
                    Console.WriteLine("It's a staff to patient!");
                }
                return ValidationResult.Success;
            }
        }
    }

}