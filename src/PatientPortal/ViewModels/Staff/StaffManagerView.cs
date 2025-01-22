using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffManagerView
    {
        public StaffSearch SearchBar { get; set; }

        public Paginator PaginationSettings { get; set; }

        public List<StaffResult> SearchResults { get; set; }
    }
    
    public class StaffResult
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public int PatientCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}