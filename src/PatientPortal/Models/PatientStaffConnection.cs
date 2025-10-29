using System;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class PatientStaffConnection
    {
        [Key]
        public int PatientStaffConnectionId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int StaffId { get; set; }

        public Patient? Patient { get; set; }
        public Staff? Staff { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
