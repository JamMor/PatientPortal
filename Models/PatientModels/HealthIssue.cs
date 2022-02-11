using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class HealthIssue
    {
        [Key]
        public int HealthIssueId { get; set; }
        
        [Required]
        [MaxLength(30)]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    //Relationship Properties=============
        public int PatientId { get;set; }
        public Patient Patient { get;set; }
        
        public List<VisitHealthIssueAssociation> AssociatedVisits { get;set; }
        public List<TestHealthIssueAssociation> AssociatedTestResults { get;set; }
    }
}