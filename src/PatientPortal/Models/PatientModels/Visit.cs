using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class Visit
    {
        [Key]
        public int VisitId { get; set; }
        [Required]
        [MinLength(5)]
        public string Comment { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Visit")]
        public DateTime DateOfVisit { get;set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int StaffId { get; set; }
        public Patient Patient { get; set; }
        public Staff Staff { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //Relationship Properties=============

        public List<VisitHealthIssueAssociation> AssociatedHealthIssues { get;set; }
    }
}