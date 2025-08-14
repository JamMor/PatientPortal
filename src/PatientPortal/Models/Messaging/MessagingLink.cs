#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class MessagingLink : IValidatableObject
    {
        [Key]
        public int MessagingLinkId { get; set; }

        public int? StaffId { get; set; }
        public int? PatientId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Relationship Properties=============

        [ForeignKey("StaffId")]
        public Staff? Staff { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public List<Unread> UnreadMessages { get; set; } = [];
        public List<ConversationParticipant> ParticipatingConversations { get; set; } = [];

        [NotMapped]
        public string UserType
        {
            get => StaffId == null ? "Patient" : "Staff";
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StaffId == null && PatientId == null)
            {
                yield return new ValidationResult(
                    "MessengerLink must have either a StaffId or a PatientId.",
                    new[] { "StaffId", "PatientId" }
                );
            }
            if (StaffId != null && PatientId != null)
            {
                yield return new ValidationResult(
                    "MessengerLink cannot have both a StaffId and a PatientId.",
                    new[] { "StaffId", "PatientId" }
                );
            }
        }
    }
}
