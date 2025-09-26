using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffManagerView
    {
        public required StaffSearch SearchBar { get; set; }

        public required Paginator PaginationSettings { get; set; }

        public List<StaffResult> SearchResults { get; set; } = [];
    }

    public class StaffResult
    {
        public int StaffId { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public int PatientCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
