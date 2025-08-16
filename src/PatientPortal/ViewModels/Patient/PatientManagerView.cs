#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientManagerView
    {
        public required PatientSearch SearchBar { get; set; }

        public required Paginator PaginationSettings { get; set; }

        public List<PatientResult> SearchResults { get; set; } = [];
    }

    public class PatientResult
    {
        public int PatientId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public required string Last4SSN { get; set; }
    }
}
